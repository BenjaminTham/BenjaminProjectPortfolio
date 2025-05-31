namespace Ecommerce.Application.Dto;
public class SecurityConfigurationDto
{
    public bool IsPasswordRequireDigit { get; set; }
    public bool IsPasswordRequireLowercase { get; set; }
    public bool IsPasswordRequireUppercase { get; set; }
    public bool IsPasswordRequireNonAlphanumeric { get; set; }
    public int PasswordRequiredLength { get; set; }
    public bool IsUserLockoutEnabled { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public int UserLockoutTime { get; set; }
}