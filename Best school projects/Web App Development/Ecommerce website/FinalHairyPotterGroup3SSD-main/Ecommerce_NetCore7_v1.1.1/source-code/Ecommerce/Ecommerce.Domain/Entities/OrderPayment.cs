using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;

public class OrderPayment : BaseEntity
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; }
    public string PaymentType { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}

