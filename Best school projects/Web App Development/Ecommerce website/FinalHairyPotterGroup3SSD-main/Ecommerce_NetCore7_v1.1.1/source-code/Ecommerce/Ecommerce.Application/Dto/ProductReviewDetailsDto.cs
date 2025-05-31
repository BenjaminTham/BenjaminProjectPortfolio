namespace Ecommerce.Application.Dto;

public class ProductReviewDetailsDto
{
    public long Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public DateTime DateCommented { get; set; }
    public string? Reply { get; set; }
    public string? RepliedBy { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DateReplied { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductPreview { get; set; }
}
