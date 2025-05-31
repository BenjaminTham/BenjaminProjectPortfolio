namespace Ecommerce.Application.Dto;

public class CustomerProfileDto
{
    public CustomerDto? CustomerInfo { get; set; }
    public int TotalOrder { get; set; }
    public decimal TotalOrderAmount { get; set; }
    public decimal TotalDeliveryCharge { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
}