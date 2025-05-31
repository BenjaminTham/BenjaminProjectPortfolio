using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Infrastructure.Sql.Identity.Filters;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
