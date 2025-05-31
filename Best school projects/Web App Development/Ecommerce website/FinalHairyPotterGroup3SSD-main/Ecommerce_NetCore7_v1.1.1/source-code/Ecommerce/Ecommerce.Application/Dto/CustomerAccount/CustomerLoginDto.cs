using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;
public class CustomerLoginDto
{
    [Required(ErrorMessage = "Username Required!")]
    [DataType(DataType.Text)]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password Required!")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}