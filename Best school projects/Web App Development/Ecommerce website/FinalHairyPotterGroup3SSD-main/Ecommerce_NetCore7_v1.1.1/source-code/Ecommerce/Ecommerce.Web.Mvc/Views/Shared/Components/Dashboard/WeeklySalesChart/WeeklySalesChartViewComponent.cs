using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.Dashboard.WeeklySalesChart;

public class WeeklySalesChartViewComponent : ViewComponent
{
    private readonly IMediator _mediator;
    public WeeklySalesChartViewComponent(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        //var featureProduct = await _mediator.Send(new GetWeeklyItemSalesCountQuery());
        return View();
    }
}
