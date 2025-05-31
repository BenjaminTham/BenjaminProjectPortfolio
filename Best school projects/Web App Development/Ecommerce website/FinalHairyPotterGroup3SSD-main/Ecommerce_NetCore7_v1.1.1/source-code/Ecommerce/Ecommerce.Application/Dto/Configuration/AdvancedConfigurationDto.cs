namespace Ecommerce.Application.Dto;
public class AdvancedConfigurationDto
{
    public bool EnableTwoFactorAuthentication { get; set; }
    public bool ActiveResetPassword { get; set; }
    public bool EnableEmailConfirmation { get; set; }
    public bool IsComingSoonEnabled { get; set; }
    public string? RoleName { get; set; }
}