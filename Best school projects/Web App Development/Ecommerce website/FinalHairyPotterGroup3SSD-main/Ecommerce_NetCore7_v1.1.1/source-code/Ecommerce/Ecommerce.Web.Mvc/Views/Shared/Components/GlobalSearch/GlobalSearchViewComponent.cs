using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.GlobalSearch;

public class GlobalSearchViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
