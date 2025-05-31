using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.PageSection;

public class PageSectionViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string action)
    {
        ViewBag.PageSectionAction = action;
        return View();
    }
}
