namespace Ecommerce.Domain.Models;
public class SecurityConfiguration
{
    public bool IsPasswordRequireDigit { get; set; } = false;
    public bool IsPasswordRequireLowercase { get; set; } = false;
    public bool IsPasswordRequireUppercase { get; set; } = false;
    public bool IsPasswordRequireNonAlphanumeric { get; set; } = false;
    public int PasswordRequiredLength { get; set; } = 1;
    public bool IsUserLockoutEnabled { get; set; } = false;
    public int MaxFailedAccessAttempts { get; set; } = 9999;
    public int UserLockoutTime { get; set; } = 0;
}

