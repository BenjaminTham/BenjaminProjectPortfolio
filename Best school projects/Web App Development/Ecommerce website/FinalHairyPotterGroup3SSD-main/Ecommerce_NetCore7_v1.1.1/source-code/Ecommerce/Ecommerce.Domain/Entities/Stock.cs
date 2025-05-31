using Ecommerce.Domain.Common;
using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;
public class Stock : BaseEntity
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public Variant? Variant { get; set; }
    public int Qty { get; set; }
    public string? Description { get; set; }
    public StockInputType StockInputType { get; set; }
}