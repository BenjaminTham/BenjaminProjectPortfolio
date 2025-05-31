namespace Ecommerce.Application.Dto;

public class ProductLiveSearchDto
{
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSlug { get; set; }
    public string? ProductCategory { get; set; }
    public string? ImagePreview { get; set; }
}
