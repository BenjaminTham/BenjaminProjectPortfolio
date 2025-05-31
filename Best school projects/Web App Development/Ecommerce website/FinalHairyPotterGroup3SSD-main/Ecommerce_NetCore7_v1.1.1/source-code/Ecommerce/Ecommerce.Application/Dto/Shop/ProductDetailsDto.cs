using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Dto;

public class ProductDetailsDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public string VariableTheme { get; set; }
    public int CategoryId { get; set; }
    public string ProductImage { get; set; }
    public string ProductImagePreview { get; set; }
    public string Price { get; set; }
    public ProductDetailsVariantDto FeatureVariant { get; set; }
    public List<ProductDetailsVariantDto> Variant { get; set; }
    public List<ColorDto> AvailableColorVariant { get; set; }
    public List<SizeDto> AvailableSizesVariant { get; set; }
    public List<ProductReviewsDto> CustomerReviews { get; set; }
}

public class ProductDetailsVariantDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long ProductId { get; set; }
    public Size Size { get; set; }
    public Color Color { get; set; }
    public string VariantImagePreview { get; set; }
    public string Sku { get; set; }
    public int? Quantity { get; set; }
    public decimal Price { get; set; }
}

public class ProductReviewsDto
{
    public long Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public string CustomerName { get; set; }
    public DateTime DateCommented { get; set; }
    public string Reply { get; set; }
    public DateTime DateReplied { get; set; }
}
