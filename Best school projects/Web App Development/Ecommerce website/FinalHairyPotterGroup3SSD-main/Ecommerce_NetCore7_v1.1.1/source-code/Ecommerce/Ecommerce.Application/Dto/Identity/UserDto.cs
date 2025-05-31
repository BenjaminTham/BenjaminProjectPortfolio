using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Dto;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age => DateOfBirth?.Year is int birthYear ? DateTime.Today.Year - birthYear : null;
    public string? Gender { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
    public Customer? Customer { get; set; }
}
