using Ecommerce.Application.Handlers.Orders.Queries;
using Ecommerce.Application.Handlers.OrderStatuses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[AllowAnonymous]
public class TrackOrderController : Controller
{
    private readonly IMediator _mediator;
    public TrackOrderController(IMediator mediator)
    {
        _mediator = mediator;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    //[Route("trackorder")]
    //[Route("TrackOrder/Index")]
    public async Task<IActionResult> Index(string invoiceNo)
    {
        if (invoiceNo == null) { return View(); }
        ViewBag.invoiceNo = invoiceNo;

        var order = await _mediator.Send(new GetOrderByInvoiceNoQuery { InvoiceNo = invoiceNo });
        if (order == null) { return View(); }

        var orderStatus = await _mediator.Send(new GetOrderStatusByIdQuery { OrderId = order.Id });
        ViewBag.CurrentOrderStatus = orderStatus.OrderByDescending(o => o.Id).Select(o => o.OrderStatusValue.StatusValue).FirstOrDefault();
        return View(orderStatus);
    }
}
