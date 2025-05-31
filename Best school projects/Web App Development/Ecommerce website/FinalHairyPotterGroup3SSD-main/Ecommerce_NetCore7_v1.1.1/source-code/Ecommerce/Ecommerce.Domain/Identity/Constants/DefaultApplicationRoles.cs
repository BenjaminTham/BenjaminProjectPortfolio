using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.Domain.Identity.Constants;
public static class DefaultApplicationRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Moderator = "Moderator";
    public const string Basic = "Basic";
    public const string Customer = "Customer";

    public static List<IdentityRole> GetDefaultRoles()
    {
        var roles = new List<IdentityRole>
        {
            new(SuperAdmin),
            new(Admin),
            new(Moderator),
            new(Basic),
            new(Customer)
        };
        return roles;
    }

    public static List<Claim> GetDefaultRoleClaims()
    {
        var roles = GetDefaultRoles();
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role.Name)).ToList();
        return claims;
    }
}