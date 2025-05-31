namespace Ecommerce.Web.Mvc.Models;

public class NavCategoryVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int? ParentCategoryId { get; set; }
    public List<NavCategoryVM> Children { get; set; }
}
