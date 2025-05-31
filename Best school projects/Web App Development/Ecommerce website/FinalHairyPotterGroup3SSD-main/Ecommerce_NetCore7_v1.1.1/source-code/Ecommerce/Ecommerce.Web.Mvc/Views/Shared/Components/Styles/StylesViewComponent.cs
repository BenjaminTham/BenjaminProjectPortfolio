using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.Styles;

public class StylesViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
