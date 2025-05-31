using System.Security.Claims;

namespace Ecommerce.Domain.Identity.Permissions;
public interface IPermissionHelper
{
    List<Claim> GetAllPermissions();
}