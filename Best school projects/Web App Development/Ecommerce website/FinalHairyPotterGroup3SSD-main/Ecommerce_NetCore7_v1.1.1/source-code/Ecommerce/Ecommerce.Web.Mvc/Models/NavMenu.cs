using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Web.Mvc.Models;

public class NavMenu
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    [ForeignKey("ParentNavMenu")]
    public int? ParentMenuId { get; set; }
    public virtual NavMenu ParentNavMenu { get; set; }
    public string MenuSection { get; set; }
    public string Area { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string MenuIcon { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
}

public class NavMenuRenderViewModel
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    [ForeignKey("ParentNavMenu")]
    public int? ParentMenuId { get; set; }
    public virtual NavMenu ParentNavMenu { get; set; }
    public string MenuSection { get; set; }
    public string Area { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string MenuIcon { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public List<NavMenuRenderViewModel> ChildMenu { get; set; }
}

public static class MenuSection
{
    public const string Main = "Main";
    public const string Options = "Options";
    public const string Configuration = "Configuration";
}
