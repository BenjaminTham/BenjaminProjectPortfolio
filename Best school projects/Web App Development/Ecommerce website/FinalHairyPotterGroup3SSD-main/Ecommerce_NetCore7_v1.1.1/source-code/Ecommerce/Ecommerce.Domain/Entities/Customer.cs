using System.ComponentModel.DataAnnotations;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;

namespace Ecommerce.Domain.Entities;
public class Customer : BaseEntity
{
    public long Id { get; set; }
    [Required]
    public string ApplicationUserId { get; set; }
    public ApplicationUser User { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public List<Order> Orders { get; set; } = new List<Order>();
    public List<CustomerReview> CustomerReviews { get; set; } = new List<CustomerReview>();
}