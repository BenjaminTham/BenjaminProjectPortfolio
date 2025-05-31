using Ecommerce.Application.Handlers.Configuration.Queries;
using Ecommerce.Application.Handlers.RenderItems.Queries;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.FeaturedProduct;


public class FeaturedProductViewComponent : ViewComponent
{
    private readonly IKeyAccessor _keyAccessor;
    private readonly IMediator _mediator;

    public FeaturedProductViewComponent(IKeyAccessor keyAccessor, IMediator mediator)
    {
        _keyAccessor = keyAccessor;
        _mediator = mediator;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var featureProduct = await _mediator.Send(new GetFeatureProductQuery());
        var dealOfTheDay = await _mediator.Send(new GetDealOfTheDayConfigQuery());

        ViewBag.FeatureProduct = featureProduct.ToList();
        ViewBag.DealOfTheDay = dealOfTheDay;
        return View();
    }
}
