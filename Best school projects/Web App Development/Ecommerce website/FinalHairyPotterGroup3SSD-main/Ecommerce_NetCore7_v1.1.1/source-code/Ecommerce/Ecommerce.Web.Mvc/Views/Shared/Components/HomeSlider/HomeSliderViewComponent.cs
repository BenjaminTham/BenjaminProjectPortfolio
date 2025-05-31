using Ecommerce.Application.Handlers.RenderItems.Queries;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.HomeSlider;

public class HomeSliderViewComponent : ViewComponent
{
    private readonly IKeyAccessor _keyAccessor;
    private readonly IMediator _mediator;

    public HomeSliderViewComponent(IKeyAccessor keyAccessor, IMediator mediator)
    {
        _keyAccessor = keyAccessor;
        _mediator = mediator;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var headerSlider = await _mediator.Send(new GetHeaderSliderQuery());
        ViewBag.HeaderSlider = headerSlider;
        return View();
    }
}
