namespace Ecommerce.Domain.Models;
public class DealOfTheDay
{
    public string? Title { get; set; }
    public long ProductId { get; set; }
    public decimal ActualPrice { get; set; }
    public decimal OfferPrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public bool IsActive { get; set; }
}