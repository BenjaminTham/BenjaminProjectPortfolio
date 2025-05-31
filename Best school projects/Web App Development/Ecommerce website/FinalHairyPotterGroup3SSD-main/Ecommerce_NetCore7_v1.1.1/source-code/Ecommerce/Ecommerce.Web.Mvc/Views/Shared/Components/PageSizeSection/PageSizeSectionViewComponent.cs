using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.PageSizeSection;

public class PageSizeSectionViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string action)
    {
        ViewBag.PageSizeSectionAction = action;
        return View();
    }
}
