using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Models;

namespace Ecommerce.Web.Mvc.Helpers;

public class NavigationMenuHelper
{
    public List<NavMenu> NavMenuBuilder()
    {
        var navmenu = new List<NavMenu>();

        // Main Categories
        navmenu.Add(new NavMenu { Id = 1, ParentMenuId = 17, Name = "Categories", Key = Permissions.Permissions_Category_View, ControllerName = "Categories", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-code-branch" });
        navmenu.Add(new NavMenu { Id = 4, ParentMenuId = 17, Name = "Products", Key = Permissions.Permissions_Product_View, ControllerName = "Product", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-box" });
       // navmenu.Add(new NavMenu { Id = 2, ParentMenuId = 17, Name = "Color", Key = Permissions.Permissions_Color_View, ControllerName = "Color", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-palette" });
       // navmenu.Add(new NavMenu { Id = 3, ParentMenuId = 17, Name = "Size", Key = Permissions.Permissions_Size_View, ControllerName = "Size", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-weight-hanging" });
        //User Adminstration
        navmenu.Add(new NavMenu { Id = 6, ParentMenuId = null, Name = "Administrator", Key = null, ControllerName = null, ActionName = null, IsActive = true, IsVisible = true, MenuIcon = "fas fa-user-shield" });
        navmenu.Add(new NavMenu { Id = 7, ParentMenuId = 6, Name = "User", Key = Permissions.Permissions_User_View, ControllerName = "User", ActionName = "Index", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 8, ParentMenuId = 6, Name = "Role", Key = Permissions.Permissions_Role_View, ControllerName = "Role", ActionName = "Index", IsActive = true, IsVisible = true });

        //Gallery
        navmenu.Add(new NavMenu { Id = 5, ParentMenuId = null, Name = "Gallery", Key = Permissions.Permissions_Gallery_Manage, ControllerName = "Gallery", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-images" });


        // Orders
        navmenu.Add(new NavMenu { Id = 14, ParentMenuId = null, Name = "Manage Order", Key = null, ControllerName = null, ActionName = null, IsActive = true, IsVisible = true, MenuIcon = "fas fa-cubes" });
        navmenu.Add(new NavMenu { Id = 13, ParentMenuId = 14, Name = "All Orders", Key = Permissions.Permissions_Order_View, ControllerName = "Order", ActionName = "Index", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 15, ParentMenuId = 14, Name = "Pending Orders", Key = Permissions.Permissions_Order_View, ControllerName = "Order", ActionName = "Pending", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 16, ParentMenuId = 14, Name = "Cancelled Orders", Key = Permissions.Permissions_Order_View, ControllerName = "Order", ActionName = "Cancelled", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 30, ParentMenuId = 14, Name = "Delivered Orders", Key = Permissions.Permissions_Order_View, ControllerName = "Order", ActionName = "Delivered", IsActive = true, IsVisible = true });

        // Miscellaneous
        navmenu.Add(new NavMenu { Id = 50, ParentMenuId = null, Name = "Miscellaneous", Key = null, ControllerName = null, ActionName = null, IsActive = true, IsVisible = true, MenuIcon = "fas fa-cubes" });
        navmenu.Add(new NavMenu { Id = 9, ParentMenuId = 50, Name = "Order Status", Key = Permissions.Permissions_OrderStatus_View, ControllerName = "OrderStatusValue", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-shopping-bag" });
        navmenu.Add(new NavMenu { Id = 10, ParentMenuId = 50, Name = "Delivery Method", Key = Permissions.Permissions_DeliveryMethod_View, ControllerName = "DeliveryMethod", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-truck" });
        navmenu.Add(new NavMenu { Id = 11, ParentMenuId = 50, Name = "Customer", Key = Permissions.Permissions_Customer_View, ControllerName = "Customer", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-users" });
        navmenu.Add(new NavMenu { Id = 12, ParentMenuId = 50, Name = "Customer Review", Key = Permissions.Permissions_ProductReview_View, ControllerName = "CustomerReview", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-star-half-alt" });


        navmenu.Add(new NavMenu { Id = 17, ParentMenuId = null, Name = "Inventory", Key = null, ControllerName = null, ActionName = null, IsActive = true, IsVisible = true, MenuIcon = "fas fa-luggage-cart" });
        navmenu.Add(new NavMenu { Id = 18, ParentMenuId = 17, Name = "Manage Stock", Key = Permissions.Permissions_Inventory_AddStock, ControllerName = "Stock", ActionName = "Index", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 19, ParentMenuId = 17, Name = "Stock History", Key = Permissions.Permissions_Inventory_StockHistory, ControllerName = "Stock", ActionName = "EntryHistory", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 32, ParentMenuId = 17, Name = "Low Stock", Key = Permissions.Permissions_Order_View, ControllerName = "Stock", ActionName = "LowStockProduct", IsActive = true, IsVisible = true });


        navmenu.Add(new NavMenu { Id = 20, ParentMenuId = null, Name = "Site Configuration", Key = null, ControllerName = null, ActionName = null, IsActive = true, IsVisible = true, MenuIcon = "fas fa-solar-panel" });
        navmenu.Add(new NavMenu { Id = 21, ParentMenuId = 20, Name = "General Config.", Key = Permissions.Permissions_Configuration_General, ControllerName = "Configuration", ActionName = "Index", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 22, ParentMenuId = 20, Name = "Shop Config.", Key = Permissions.Permissions_Configuration_Shop, ControllerName = "Configuration", ActionName = "Shop", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 23, ParentMenuId = 20, Name = "Social", Key = Permissions.Permissions_Configuration_Social, ControllerName = "Configuration", ActionName = "Social", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 24, ParentMenuId = 20, Name = "Basic SEO", Key = Permissions.Permissions_Configuration_BasicSeo, ControllerName = "Configuration", ActionName = "BasicSeo", IsActive = true, IsVisible = true });


        navmenu.Add(new NavMenu { Id = 25, ParentMenuId = null, Name = "Settings", Key = null, ControllerName = null, ActionName = null, IsActive = true, IsVisible = true, MenuIcon = "fas fa-cog fa-spin" });
        navmenu.Add(new NavMenu { Id = 26, ParentMenuId = 25, Name = "Stock Settings", Key = Permissions.Permissions_Configuration_Stock, ControllerName = "Settings", ActionName = "Stock", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 27, ParentMenuId = 25, Name = "SMTP Settings", Key = Permissions.Permissions_Configuration_Smtp, ControllerName = "Settings", ActionName = "Smtp", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 28, ParentMenuId = 25, Name = "Security", Key = Permissions.Permissions_Configuration_Security, ControllerName = "Settings", ActionName = "Security", IsActive = true, IsVisible = true });
        navmenu.Add(new NavMenu { Id = 29, ParentMenuId = 25, Name = "Advanced", Key = Permissions.Permissions_Configuration_Advanced, ControllerName = "Settings", ActionName = "Advanced", IsActive = true, IsVisible = true });


        navmenu.Add(new NavMenu { Id = 5, ParentMenuId = null, Name = "Contact Query", Key = Permissions.Permissions_ContactQuery_View, ControllerName = "ContactQuery", ActionName = "Index", IsActive = true, IsVisible = true, MenuIcon = "fas fa-envelope-open-text" });

        return navmenu;
    }






    public List<NavMenuRenderViewModel> GetNavMenu(List<string> claims)
    {
        var navmenu = this.NavMenuBuilder();


        var menubyclaim = navmenu.Where(o => claims.Contains(o.Key) || (o.Key == null && o.ActionName != null)).ToList();
        var menubyparent = navmenu.Where(o => menubyclaim.Select(o => o.ParentMenuId).ToList().Contains(o.Id)).ToList();

        menubyclaim.AddRange(menubyparent);

        List<NavMenuRenderViewModel> listMenu = menubyclaim
                          .Select(m => new NavMenuRenderViewModel()
                          {
                              Id = m.Id,
                              Name = m.Name,
                              Area = m.Area,
                              ActionName = m.ActionName,
                              ControllerName = m.ControllerName,
                              ParentMenuId = m.ParentMenuId,
                              MenuIcon = m.MenuIcon,
                              IsVisible = m.IsVisible,
                          }).Distinct().ToList();

        var d = listMenu;

        List<NavMenuRenderViewModel> menuTree = GetMenuTree(listMenu, null);

        return menuTree;
    }

    public List<NavMenuRenderViewModel> GetMenuTree(List<NavMenuRenderViewModel> list, int? parent)
    {
        return list.Where(x => x.ParentMenuId == parent && x.IsVisible == true).Select(x => new NavMenuRenderViewModel
        {
            Id = x.Id,
            Name = x.Name,
            ParentMenuId = x.ParentMenuId,
            Area = x.Area,
            ActionName = x.ActionName,
            ControllerName = x.ControllerName,
            MenuIcon = x.MenuIcon,
            IsVisible = x.IsVisible,
            ChildMenu = GetMenuTree(list, x.Id)
        }).ToList();
    }

}
