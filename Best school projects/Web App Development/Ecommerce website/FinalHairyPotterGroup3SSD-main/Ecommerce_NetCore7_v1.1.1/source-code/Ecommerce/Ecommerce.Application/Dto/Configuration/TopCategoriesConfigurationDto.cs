namespace Ecommerce.Application.Dto;
public class TopCategoriesConfigurationDto
{
    public int CategoryId { get; set; }
    public string? Slug { get; set; }
    public string? Image { get; set; }
    public string? ImagePreview { get; set; }
    public string? Title { get; set; }
    public int Order { get; set; }
}