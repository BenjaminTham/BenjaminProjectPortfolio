using Ecommerce.Application.Handlers.RenderItems.Queries;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.NewArrival;

public class NewArrivalViewComponent : ViewComponent
{
    private readonly IKeyAccessor _keyAccessor;
    private readonly IMediator _mediator;

    public NewArrivalViewComponent(IKeyAccessor keyAccessor, IMediator mediator)
    {
        _keyAccessor = keyAccessor;
        _mediator = mediator;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var newProductShowcase = await _mediator.Send(new GetNewProductQuery { itemQty = 10});
        return View(newProductShowcase);
    }
}
