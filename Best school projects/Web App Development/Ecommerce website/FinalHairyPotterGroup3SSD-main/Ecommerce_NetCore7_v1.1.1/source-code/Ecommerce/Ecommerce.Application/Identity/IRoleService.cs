using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Identity;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetRolesAsync();
    Task<Response<string>> AddRoleAsync(AddEditRoleDto addRoleDto);
    Task<Response<AddEditRoleDto>> GetRoleByIdAsync(string roleId);
    Task<Response<UserIdentityDto>> UpdateRoleAsync(AddEditRoleDto editUserDto);
    Task<Response<UserIdentityDto>> DeleteRoleAsync(string roleId);
    Task<Response<ManageRolePermissionsDto>> ManagePermissionsAsync(string roleId, string permissionValue);
    Task<Response<RoleIdentityDto>> ManageRoleClaimAsync(ManageRoleClaimDto manageRoleClaimDto);
}
