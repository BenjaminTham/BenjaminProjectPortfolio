using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class EditProfileDto
{
    public string? UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicturePreview { get; set; }
    public string? ProfilePicture { get; set; }
    [Required]
    public string Gender { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? UserFromDate { get; set; }
}
