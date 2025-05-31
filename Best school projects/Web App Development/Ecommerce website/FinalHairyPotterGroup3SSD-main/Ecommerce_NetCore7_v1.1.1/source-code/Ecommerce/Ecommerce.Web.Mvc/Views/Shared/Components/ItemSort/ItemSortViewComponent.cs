using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.ItemSort;

public class ItemSortViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string action, string sortColumn, string displayName)
    {
        ViewBag.ItemSortAction = action;
        ViewBag.ItemSortColumn = sortColumn;
        ViewBag.ItemDisplayName = displayName;
        return View();
    }
}
