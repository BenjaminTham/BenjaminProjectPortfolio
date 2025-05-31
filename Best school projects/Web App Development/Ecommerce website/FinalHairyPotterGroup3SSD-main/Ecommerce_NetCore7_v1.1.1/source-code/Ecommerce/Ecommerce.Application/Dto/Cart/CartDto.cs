namespace Ecommerce.Application.Dto;
public class CartDto
{
    public long ProductId { get; set; }
    //public long ProductSlug { get; set; }
    public long VariantId { get; set; }
    public string Title { get; set; }
    public string? Sku { get; set; }
    public string? Image { get; set; }
    public decimal Price { get; set; }
    public int StockQty { get; set; }
    public int Qty { get; set; }
    public decimal TotalPrice { get; set; }
}