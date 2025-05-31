using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class Variant : BaseEntity
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long ProductId { get; set; }
    public int? SizeId { get; set; }
    public int? ColorId { get; set; }
    public string? Sku { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public Size? Size { get; set; }
    public Color? Color { get; set; }
}