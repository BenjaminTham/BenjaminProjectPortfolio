using AutoMapper;
using Ecommerce.Application.Handlers.Customers.Commands;
using Ecommerce.Application.Handlers.Customers.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Extension;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class CustomerController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public CustomerController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Permissions.Permissions_Customer_View)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Permissions.Permissions_Customer_View)]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetCustomersWithPagingQuery() { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    [HttpPost]
    public async Task<IActionResult> CreateByUser(string userId)
    {
        var response = await _mediator.Send(new CreateCustomerByUserIdCommand { UserId = userId });
        if (response.Succeeded) return Json(response);
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }

    [Authorize(Permissions.Permissions_Customer_Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
        if (customer == null)
        {
            return NotFound();
        }

        var customerCommand = _mapper.Map<UpdateCustomerCommand>(customer);
        return View(customerCommand);
    }

    [Authorize(Permissions.Permissions_Customer_Edit)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCustomerCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded == true) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, response.Message);
        }
        return View(command);
    }

    [HttpGet]
    [Authorize(Permissions.Permissions_Customer_Edit)]
    public async Task<IActionResult> Details(int id)
    {
        if (id == null) return NotFound();
        var customerInfo = await _mediator.Send(new GetCustomerProfileByIdQuery { Id = id });

        ViewData["CustomerInfo"] = customerInfo;
        return View();
    }

}
