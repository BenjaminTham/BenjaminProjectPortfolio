using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email Required!")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
}
