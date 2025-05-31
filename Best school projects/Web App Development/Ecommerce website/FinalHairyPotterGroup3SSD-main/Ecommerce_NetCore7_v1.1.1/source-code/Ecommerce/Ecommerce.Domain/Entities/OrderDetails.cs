using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;

public class OrderDetails : BaseEntity
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; }
    public long ProductVariantId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Qty { get; set; }
}

