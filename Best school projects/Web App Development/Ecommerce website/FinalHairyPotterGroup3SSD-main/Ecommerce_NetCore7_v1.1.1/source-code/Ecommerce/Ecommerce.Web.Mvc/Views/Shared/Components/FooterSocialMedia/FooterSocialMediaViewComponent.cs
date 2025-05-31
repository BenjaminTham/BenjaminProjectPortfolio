using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.FooterSocialMedia;

public class FooterSocialMediaViewComponent : ViewComponent
{
    public FooterSocialMediaViewComponent()
    {
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
