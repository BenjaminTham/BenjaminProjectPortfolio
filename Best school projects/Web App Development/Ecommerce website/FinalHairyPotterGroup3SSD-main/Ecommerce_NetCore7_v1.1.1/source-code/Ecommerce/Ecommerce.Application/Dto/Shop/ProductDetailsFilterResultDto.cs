namespace Ecommerce.Application.Dto;

public class ProductDetailsFilterResultDto
{
    public long VariantId { get; set; }
    public string? Sku { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public string? VariantImage { get; set; }
    public int SizeId { get; set; }
    public int ColorId { get; set; }
}
