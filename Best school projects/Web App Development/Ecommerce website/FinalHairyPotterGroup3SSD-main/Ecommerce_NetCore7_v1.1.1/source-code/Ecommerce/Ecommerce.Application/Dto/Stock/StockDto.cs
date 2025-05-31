using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Dto;

public class StockDto
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public Variant? Variant { get; set; }
    public int Qty { get; set; }
    public string? Description { get; set; }
    public StockInputType StockInputType { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
