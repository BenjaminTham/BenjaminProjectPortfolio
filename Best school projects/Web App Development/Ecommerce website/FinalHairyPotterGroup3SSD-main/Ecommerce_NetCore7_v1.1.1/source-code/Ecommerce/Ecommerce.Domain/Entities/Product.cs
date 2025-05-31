using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;
public class Product : BaseEntity
{
    public long Id { get; set; }
    public string Name { get; set; } //100
    public string Slug { get; set; }
    public string? KeySpecs { get; set; }
    public string? ShortDescription { get; set; } //200
    public string? Description { get; set; }
    public string? VariableTheme { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<CustomerReview> CustomerReviews { get; set; } = new List<CustomerReview>();
}
