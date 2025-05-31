using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class CustomerReview : BaseEntity
{
    public long Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime DateCommented { get; set; }
    public string? Reply { get; set; }
    public string? RepliedBy { get; set; }
    public DateTime? DateReplied { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; }
    public bool IsActive { get; set; }
}