using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class Category : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int? ParentCategoryId { get; set; }
    public virtual Category ParentCategory { get; set; }
    public List<Category> Children { get; set; }
    public List<Product> Products { get; set; }
}
