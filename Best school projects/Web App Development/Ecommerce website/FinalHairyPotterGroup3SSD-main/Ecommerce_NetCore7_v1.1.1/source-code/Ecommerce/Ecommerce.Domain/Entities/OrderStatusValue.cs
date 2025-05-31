using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class OrderStatusValue : BaseEntity
{
    public int Id { get; set; }
    public string StatusValue { get; set; }
    public string Description { get; set; }
    public List<OrderStatus> OrderStatus { get; set; } = new List<OrderStatus>();
}