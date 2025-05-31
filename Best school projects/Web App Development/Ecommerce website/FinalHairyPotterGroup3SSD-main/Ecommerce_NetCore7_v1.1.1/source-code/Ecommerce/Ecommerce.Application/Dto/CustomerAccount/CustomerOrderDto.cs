using Ecommerce.Application.Dto;

namespace Ecommerce.Application.Dto;
public class CustomerOrderDto
{
    public long OrderId { get; set; }
    public string InvoiceNo { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal? DeliveryCharge { get; set; }
    public decimal? Total { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? OrderStatus { get; set; }
    public List<OrderDetailsDto> OrderDetails { get; set; } = new List<OrderDetailsDto>();
}