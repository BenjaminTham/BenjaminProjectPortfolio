namespace Ecommerce.Application.Dto;

public class OrderDto
{
    public long Id { get; set; }
    public string InvoiceNo { get; set; }
    public long CustomerId { get; set; }
    public decimal OrderAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public decimal? DeliveryCharge { get; set; }
    public string PaymentMethod { get; set; }
    public string CurrentOrderStatus { get; set; }
    public string CustomerName { get; set; }
    public string ShippingAddress { get; set; }
    public string? CustomerComment { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
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
