using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class TwoStepDto
{
    [Required]
    public string TwoFactorCode { get; set; }
    public string UserName { get; set; }
    public bool IsEnabled { get; set; }
    public bool RememberMe { get; set; } = false;
    public bool SendCode { get; set; } = false;
    public string ReturnUrl { get; set; }
    public bool IsEmailConfirmed { get; set; }
}
