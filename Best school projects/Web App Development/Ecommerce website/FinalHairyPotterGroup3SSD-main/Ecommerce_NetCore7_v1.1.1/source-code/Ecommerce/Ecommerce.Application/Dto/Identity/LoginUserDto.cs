using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class LoginUserDto
{
    [Required(ErrorMessage = "Username Required!")]
    [DataType(DataType.Text)]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '_' are allowed.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password Required!")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}