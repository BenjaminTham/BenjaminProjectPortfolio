namespace Ecommerce.Application.Dto;
public class FeatureProductConfigurationDto
{
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductCategory { get; set; }
    public string? ImagePreview { get; set; }
    public int Order { get; set; }
}