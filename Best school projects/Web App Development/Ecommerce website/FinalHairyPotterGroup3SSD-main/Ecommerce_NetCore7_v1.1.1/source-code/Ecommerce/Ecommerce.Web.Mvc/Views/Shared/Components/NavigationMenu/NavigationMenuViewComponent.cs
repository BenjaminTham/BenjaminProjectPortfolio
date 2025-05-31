using Ecommerce.Application.Interfaces;
using Ecommerce.Web.Mvc.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Views.Shared.Components.NavigationMenu;

public class NavigationMenuViewComponent : ViewComponent
{
    private ICurrentUser _currentUser;
    public NavigationMenuViewComponent(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var permissions = await _currentUser.Permissions();
        var per = permissions.Select(o => o.Value).ToList();
        var menu = new NavigationMenuHelper().GetNavMenu(per);

        ViewBag.NavigationMenu = menu;
        return View();
    }

}
