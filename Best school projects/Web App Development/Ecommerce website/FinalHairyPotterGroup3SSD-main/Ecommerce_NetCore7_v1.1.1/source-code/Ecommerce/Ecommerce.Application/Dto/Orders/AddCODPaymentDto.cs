namespace Ecommerce.Application.Dto;

public class AddCODPaymentDto
{
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }

}
