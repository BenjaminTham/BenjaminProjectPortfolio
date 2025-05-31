using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Domain.Identity.Permissions;
using System.Reflection;
using System.Security.Claims;

namespace Ecommerce.Infrastructure.Sql.Identity.Permission;

public class PermissionHelper : IPermissionHelper
{
    public List<Claim> GetAllPermissions()
    {
        var allPermissions = new List<Claim>();
        var permissionClass = typeof(Permissions);
        var allModulesPermissions = permissionClass.GetFields().ToList();

        var permissions = permissionClass.GetFields(BindingFlags.Static | BindingFlags.Public);
        allPermissions.AddRange(permissions.Select(permission =>
            new Claim(CustomClaimTypes.Permission, permission.GetValue(null).ToString())));
        return allPermissions;
    }
}
