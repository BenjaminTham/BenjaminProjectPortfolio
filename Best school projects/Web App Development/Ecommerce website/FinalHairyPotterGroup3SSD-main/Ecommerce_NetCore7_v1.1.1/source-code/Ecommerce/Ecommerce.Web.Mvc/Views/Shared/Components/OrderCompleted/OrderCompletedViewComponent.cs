using Ecommerce.Application.Handlers.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.OrderCompleted;

public class OrderCompletedViewComponent : ViewComponent
{
    private readonly IMediator _mediator;
    public OrderCompletedViewComponent(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<IViewComponentResult> InvokeAsync(int id)
    {
        var order = await _mediator.Send(new GetOrderInvoiceByOrderIdQuery { Id = id });
        ViewBag.OrderInvoice = order;
        return View(order);
    }
}
