using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.RenderItems.Queries;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.TopCategories;

public class TopCategoriesViewComponent : ViewComponent
{
    private readonly IKeyAccessor _keyAccessor;
    private readonly IMediator _mediator;

    public TopCategoriesViewComponent(IKeyAccessor keyAccessor, IMediator mediator)
    {
        _keyAccessor = keyAccessor;
        _mediator = mediator;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var conStock = _keyAccessor?["BannerConfiguration"] is not null
            ? JsonSerializer.Deserialize<List<BannerDto>>(_keyAccessor.GetSection("BannerConfiguration"))!
            : new List<BannerDto>();


        var topCategories = await _mediator.Send(new GetTopCategoriesQuery());
        ViewBag.TopCategories = topCategories;
        ViewBag.BannerList = conStock.Where(c => c.IsActive).Take(2).ToList();

        return View();
    }
}
