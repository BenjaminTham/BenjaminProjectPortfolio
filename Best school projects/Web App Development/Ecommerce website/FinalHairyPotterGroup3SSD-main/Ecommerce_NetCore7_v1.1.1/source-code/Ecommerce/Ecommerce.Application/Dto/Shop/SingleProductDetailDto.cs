namespace Ecommerce.Application.Dto;

public class SingleProductDetailDto
{
    public long ProductId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? VariableTheme { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? ProductImage { get; set; }
    public string? ProductImagePreview { get; set; }
    public SingleProductDetailVariantDto? FeatureVariant { get; set; }
    public IList<ColorDto> AvailableColorVariant { get; set; }
    public IList<SizeDto> AvailableSizesVariant { get; set; }
}

public class SingleProductDetailVariantDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public long ProductId { get; set; }
    public int? SizeId { get; set; }
    public string? SizeName { get; set; }
    public int? ColorId { get; set; }
    public string? ColorName { get; set; }
    public string? VariantImagePreview { get; set; }
    public string? Sku { get; set; }
    public int? Qty { get; set; }
    public decimal Price { get; set; }
}