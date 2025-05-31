using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Domain.Identity.Entities;
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = false;
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    public Customer? Customer { get; set; }
}