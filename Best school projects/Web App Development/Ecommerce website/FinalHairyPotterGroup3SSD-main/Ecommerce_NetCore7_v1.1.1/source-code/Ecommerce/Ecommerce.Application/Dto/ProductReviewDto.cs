namespace Ecommerce.Application.Dto;

public class ProductReviewDto
{
    public long Id { get; set; }
    public string InvoiceNo { get; set; }
    public long OrderId { get; set; }
    
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? DateCommented { get; set; }
    public string? Reply { get; set; }
    public string? RepliedBy { get; set; }
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSlug { get; set; }
    public string? ProductImage { get; set; }
    public long CustomerId { get; set; }
    public string? CustomerName { get; set; }
    
    public bool? IsActive { get; set; }
    public string? PaymentStatus { get; set; }
    public string? OrderStatusValue { get; set; }
}
