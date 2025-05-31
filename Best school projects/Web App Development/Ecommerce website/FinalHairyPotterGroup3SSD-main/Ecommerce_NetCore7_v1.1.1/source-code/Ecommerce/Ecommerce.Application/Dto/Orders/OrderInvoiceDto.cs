namespace Ecommerce.Application.Dto;

public class OrderInvoiceDto
{
    public long OrderId { get; set; }
    public string InvoiceNo { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string ShippingAddress { get; set; }
    public string OrderDate { get; set; }
    public string PaymentMethod { get; set; }
    public string Subtotal { get; set; }
    public string DeliveryCharge { get; set; }
    public string TotalAmount { get; set; }
    public List<OrderInvoiceOrderItems> OrderItems { get; set; }
}



public class OrderInvoiceOrderItems
{
    public string Item { get; set; }
    public decimal UnitPrice { get; set; }
    public string UnitPriceWithCurrency { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public string TotalWithCurrency { get; set; }
}
