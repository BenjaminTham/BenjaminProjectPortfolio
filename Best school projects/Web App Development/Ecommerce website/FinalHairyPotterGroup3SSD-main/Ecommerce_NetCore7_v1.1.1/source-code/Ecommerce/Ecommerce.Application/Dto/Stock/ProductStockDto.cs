namespace Ecommerce.Application.Dto;

public class ProductStockDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public string? CategoryName { get; set; }
    public string? ProductImagePreview { get; set; }
    public List<ProductVariantStockDto>? ProductVariant { get; set; }
}

public class ProductVariantStockDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long ProductId { get; set; }
    public string? VariantImagePreview { get; set; }
    public string? Sku { get; set; }
    public int? Qty { get; set; }
    public decimal Price { get; set; }
}
