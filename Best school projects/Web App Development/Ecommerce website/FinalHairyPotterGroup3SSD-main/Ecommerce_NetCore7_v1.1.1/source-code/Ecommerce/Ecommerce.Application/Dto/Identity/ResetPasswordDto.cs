using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Password is Required!")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Confirm Password is Required!")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }

}
