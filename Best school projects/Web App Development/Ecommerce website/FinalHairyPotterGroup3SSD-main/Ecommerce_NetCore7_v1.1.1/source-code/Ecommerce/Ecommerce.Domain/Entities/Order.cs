using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;

public class Order : BaseEntity
{
    public long Id { get; set; }
    public string InvoiceNo { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public decimal OrderAmount { get; set; }
    public int DeliveryMethodId { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public decimal DeliveryCharge { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    public string CustomerName { get; set; }
    public string ShippingAddress { get; set; }
    public string CustomerMobile { get; set; }
    public string CustomerEmail { get; set; }
    public string? CustomerComment { get; set; }
    //public int OrderStatusId { get; set; }

    public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    public List<OrderStatus> OrderStatus { get; set; } = new List<OrderStatus>();
    public OrderStatus? CurrentStatus => OrderStatus.OrderByDescending(c=>c.Id).FirstOrDefault();
    public OrderPayment OrderPayments { get; set; }

    public bool IsNew
    {
        get
        {
            if (CreatedDate != null)
            {
                TimeSpan difference = (TimeSpan)(DateTime.UtcNow - CreatedDate);
                return difference.TotalHours <= 24;
            }
            return false;
        }
    }
}

