namespace Ecommerce.Application.Dto;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int? ParentCategoryId { get; set; }
    public string ParentCategoryName { get; set; }
    public CategoryDto ParentCategory { get; set; }
    public List<CategoryDto> Children { get; set; }
}
