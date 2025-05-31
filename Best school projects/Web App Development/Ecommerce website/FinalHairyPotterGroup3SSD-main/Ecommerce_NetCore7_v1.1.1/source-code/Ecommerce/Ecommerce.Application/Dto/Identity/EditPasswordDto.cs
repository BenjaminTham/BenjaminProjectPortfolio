using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class EditPasswordDto
{
    public string? UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
}
