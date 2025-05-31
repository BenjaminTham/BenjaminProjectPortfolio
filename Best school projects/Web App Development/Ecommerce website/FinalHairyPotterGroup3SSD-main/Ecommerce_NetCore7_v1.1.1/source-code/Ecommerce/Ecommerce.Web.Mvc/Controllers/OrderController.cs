using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Customers.Queries;
using Ecommerce.Application.Handlers.DeliveryMethods.Queries;
using Ecommerce.Application.Handlers.Orders.Commands;
using Ecommerce.Application.Handlers.Orders.Queries;
using Ecommerce.Application.Handlers.OrderStatusValues.Queries;
using Ecommerce.Application.Identity;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    public OrderController(IMediator mediator, IMapper mapper, IUserService userService)
    {
        _mediator = mediator;
        _mapper = mapper;
        _userService = userService;
    }

    #region View
    [Authorize(Permissions.Permissions_Order_View)]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().PostPageResponse(Request);
        var result = await _mediator.Send(new GetOrdersWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });
        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }
    #endregion

    #region Checkout
    [HttpGet]
    [Route("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var customerInfo = await _mediator.Send(new GetCustomerInfoByLoginUserQuery()) ?? new CustomerDto();
        if (customerInfo?.Id <= 0)
        {
            var user = _userService.GetCurrentUsersAsync();
            customerInfo.UserFirstName = user.Result.FirstName;
            customerInfo.UserLastName = user.Result.LastName;
            customerInfo.UserEmail = user.Result.Email;
            customerInfo.UserPhoneNumber = user.Result.PhoneNumber;
        }

        ViewBag.CustomerInfo = customerInfo;
        string cookieValueFromReq = Request.Cookies["shop-cart"]!;
        if (cookieValueFromReq != null && JsonSerializer.Deserialize<List<CartDto>>(cookieValueFromReq)!.Count != 0)
        {
            var response = await _mediator.Send(new GetDeliveryMethodsQuery());
            ViewData["DeliveryMethod"] = response.Where(o => o.IsActive).ToList();
            return View();
        }
        TempData["notification"] = "<script>swal(`" + "Your Cart in Empty!" + "`, `" + "Please Add Some Items." + "`,`" + "warning" + "`)" + "</script>";
        return RedirectToAction("Index", "Shop");
    }

    [HttpPost]
    [Route("checkout")]
    public async Task<IActionResult> Checkout(CheckoutDto checkoutDto)
    {
        // Regex pattern for alphanumeric characters, spaces, and specified punctuation
        var alphaNumericPunctuation = @"^[a-zA-Z0-9\s.,!#?]*$";

        // Check all relevant fields
        bool isValid = true;

        if (!Regex.IsMatch(checkoutDto.CustomerFirstName, alphaNumericPunctuation) ||
            !Regex.IsMatch(checkoutDto.CustomerLastName, alphaNumericPunctuation) ||
            !Regex.IsMatch(checkoutDto.ShippingAddress, alphaNumericPunctuation) ||
            (checkoutDto.CustomerComment != null && !Regex.IsMatch(checkoutDto.CustomerComment, alphaNumericPunctuation)))
        {
            isValid = false;
        }

        if (!isValid)
        {
            ModelState.AddModelError("ShippingInfo", "Invalid characters in shipping information. " +
                "Only alphanumeric characters, spaces, '.', ',', '!', and '?, #' are allowed.");
        }

        if (!ModelState.IsValid)
        {
            return Json(new JsonResponse
            {
                Success = false,
                Message = ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage
            });
        }

        // Proceed if ModelState is valid
        var response = await _mediator.Send(new CreateOrderCheckoutCommand { Checkout = checkoutDto });
        if (response.Succeeded)
        {
            TempData["OrderId"] = Convert.ToInt32(response.Message);
            return Json(new JsonResponse { Success = true, Data = response.Message });
        }

        return Json(new JsonResponse { Success = false, Message = response.Message });
    }

    #endregion

    public IActionResult Invoice() => View();

    #region OrderCompleted
    [HttpGet]
    [Route("order-completed")]
    public async Task<IActionResult> OrderCompleted()
    {
        var orderId = Convert.ToInt32(TempData["OrderId"]);
        if (orderId > 0)
        {
            var order = await _mediator.Send(new GetOrderInvoiceByOrderIdQuery { Id = orderId });
            ViewBag.OrderInvoice = order;
            return View();
        }
        return RedirectToAction("Index", "Home");
    }
    #endregion

    #region Order Details
    [HttpGet]
    [Authorize(Permissions.Permissions_Order_View)]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return NotFound();
        var orderDetails = await _mediator.Send(new GetOrderDetailsByIdQuery { OrderId = id });
        ViewData["OrderStatusId"] = new SelectList(await _mediator.Send(new GetOrderStatusValuesQuery()), "Id", "StatusValue", orderDetails?.CurrentOrderStatus?.OrderStatusValue?.Id);
        return View(orderDetails);
    }
    #endregion

    #region UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_Order_View)]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusDto updateOrderStatus)
    {
        if (!ModelState.IsValid) return View(updateOrderStatus);

        if (updateOrderStatus?.OrderId == null) return NotFound();
        ViewData["OrderStatusId"] = new SelectList(await _mediator.Send(new GetOrderStatusValuesQuery()), "Id", "StatusValue", updateOrderStatus.NewOrderStatus);
        var response = await _mediator.Send(new UpdateOrderStatusCommand { UpdateOrderStatus = updateOrderStatus });
        if (response.Succeeded) return Json("success");
        return Json("failed");
    }
    #endregion

    #region UpdateShippingInfo
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_Order_View)]
    public async Task<IActionResult> UpdateShippingInfo(UpdateOrderShippingInfoDto updateOrderShippingInfo)
    {
        if (!ModelState.IsValid) return View(updateOrderShippingInfo);

        if (updateOrderShippingInfo?.OrderId == null) return NotFound();
        var response = await _mediator.Send(new UpdateOrderShippingInfoCommand { UpdateOrderShipping = updateOrderShippingInfo });
        if (response.Succeeded) return Json("success");
        return Json("failed");
    }
    #endregion

    #region Add Cash On Delivery Payment
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_Order_View)]
    public async Task<IActionResult> AddCODPayment(AddCODPaymentDto addCodPayment)
    {
        if (addCodPayment?.OrderId == null) return NotFound();
        var response = await _mediator.Send(new CreateCODPaymentCommand { AddCODPayment = addCodPayment });
        if (response.Succeeded) return Json("success");
        return Json("failed");
    }
    #endregion

    #region Pending
    [Authorize(Permissions.Permissions_Order_View)]
    public IActionResult Pending() => View();

    [HttpPost]
    public async Task<IActionResult> PendingRenderView()
    {
        var paging = new PageRequest().PostPageResponse(Request);
        var result = await _mediator.Send(new GetPendingOrdersWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });
        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }
    #endregion

    #region Cancelled
    [Authorize(Permissions.Permissions_Order_View)]
    public IActionResult Cancelled() => View();

    [HttpPost]
    public async Task<IActionResult> CancelledRenderView()
    {
        var paging = new PageRequest().PostPageResponse(Request);
        var result = await _mediator.Send(new GetCancelledOrdersWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });
        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }
    #endregion

    #region Delivered
    [Authorize(Permissions.Permissions_Order_View)]
    public IActionResult Delivered() => View();

    [HttpPost]
    public async Task<IActionResult> DeliveredRenderView()
    {
        var paging = new PageRequest().PostPageResponse(Request);
        var result = await _mediator.Send(new GetDeliveredOrdersWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });
        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }
    #endregion

    //#region Completed
    //[Authorize(Permissions.Permissions_Order_View)]
    //public IActionResult Completed() => View();

    //[HttpPost]
    //public async Task<IActionResult> CompletedRenderView()
    //{
    //    var paging = new PageRequest().PostPageResponse(Request);
    //    var result = await _mediator.Send(new GetCompletedOrdersWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });
    //    var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
    //    return Json(jsonData);
    //}
    //#endregion

}
