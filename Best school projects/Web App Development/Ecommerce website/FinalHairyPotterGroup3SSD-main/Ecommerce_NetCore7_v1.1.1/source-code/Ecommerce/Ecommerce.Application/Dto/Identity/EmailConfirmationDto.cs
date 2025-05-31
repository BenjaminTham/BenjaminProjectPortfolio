using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class EmailConfirmationDto
{
    [Required]
    public string ConfirmationCode { get; set; }
    public string UserName { get; set; }
    public string ReturnUrl { get; set; }
}
