using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Dto;
public class ProductStockHistoryDto
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public string? VariantTitle { get; set; }
    public int Qty { get; set; }
    public string? Description { get; set; }
    public StockInputType StockInputType { get; set; }
    public string StockInputTypeName { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
