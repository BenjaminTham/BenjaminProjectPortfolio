namespace Ecommerce.Application.Dto;

public class OrderItemDto
{
    public long OrderId { get; set; }
    public string Item { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total => UnitPrice * Quantity;
}
