using AutoMapper;
using Ecommerce.Application.Handlers.OrderStatusValues.Commands;
using Ecommerce.Application.Handlers.OrderStatusValues.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class OrderStatusValueController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public OrderStatusValueController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    [Authorize(Permissions.Permissions_OrderStatus_View)]
    public IActionResult Index()
    {
        return View();
    }
    [Authorize(Permissions.Permissions_OrderStatus_View)]
    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetOrderStatusValueWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }
    [Authorize(Permissions.Permissions_OrderStatus_Create)]
    public IActionResult Create()
    {
        return View();
    }
    [Authorize(Permissions.Permissions_OrderStatus_Create)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderStatusValueCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction(nameof(Index));
        }

        return View(command);
    }
    [Authorize(Permissions.Permissions_OrderStatus_Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _mediator.Send(new GetOrderStatusValueByIdQuery { Id = id });
        if (response == null)
        {
            return NotFound();
        }

        var deliveryMethodCommand = _mapper.Map<UpdateOrderStatusValueCommand>(response);
        return View(deliveryMethodCommand);
    }
    [Authorize(Permissions.Permissions_OrderStatus_Edit)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateOrderStatusValueCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        return View(command);
    }

    [Authorize(Permissions.Permissions_OrderStatus_Delete)]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteOrderStatusValueCommand { Id = id });
        return Json(response);
    }
}
