using AutoMapper;
using Ecommerce.Application.Handlers.DeliveryMethods.Commands;
using Ecommerce.Application.Handlers.DeliveryMethods.Queries;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;
[Authorize]
public class DeliveryMethodController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public DeliveryMethodController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    [Authorize(Permissions.Permissions_OrderStatus_View)]
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> RenderView()
    {
        var paging = new PageRequest().GetPageResponse(Request);
        var result = await _mediator.Send(new GetDeliveryMethodsWithPagingQuery { page = paging.PageIndex, length = paging.Length, searchValue = paging.SearchValue, sortColumn = paging.SortColumnName, sortOrder = paging.SortOrder });

        var jsonData = new { data = result.Items, draw = paging.Draw, recordsFiltered = result.TotalCount, recordsTotal = result.TotalCount };
        return Json(jsonData);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDeliveryMethodCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction(nameof(Index));
        }

        return View(command);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var response = await _mediator.Send(new GetDeliveryMethodByIdQuery { Id = id });
        if (response == null)
        {
            return NotFound();
        }

        var deliveryMethodCommand = _mapper.Map<UpdateDeliveryMethodCommand>(response);
        return View(deliveryMethodCommand);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDeliveryMethodCommand command)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        return View(command);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteDeliveryMethodCommand { Id = id });
        return Json(response);
    }
}
