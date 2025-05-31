using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;
using System.Security.Claims;

namespace Ecommerce.Application.Identity;

public interface IUserService
{
    Task<Response<UserIdentityDto>> AddUserAsync(AddEditUserDto addEditUserDto);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetCurrentUsersAsync();
    Task<Response<AddEditUserDto>> GetUserByIdAsync(string userId);
    Task<Response<AddEditUserDto>> GetUserByUserNameAsync(string username);
    Task<Response<AddEditUserDto>> GetUserByEmailAsync(string Email);
    Task<Response<UserIdentityDto>> UpdateUserAsync(AddEditUserDto addEditUserDto);
    Task<Response<UserIdentityDto>> DeleteUserAsync(string userId);

    Task<Response<IList<Claim>>> GetAllClaims(ClaimsPrincipal claimsPrincipal);
    Task<Response<IList<string>>> GetRolesAsync(ClaimsPrincipal claimsPrincipal);
    Task<Response<UserRolesDto>> ManageRolesAsync(string userId);
    Task<Response<UserIdentityDto>> ManageRolesAsync(ManageUserRolesDto manageUserRolesDto);
    Task<Response<ManageUserPermissionsDto>> ManagePermissionsAsync(string userId, string permissionValue);
    Task<Response<UserIdentityDto>> ManageUserClaimAsync(ManageUserClaimDto manageUserClaimDto);
}
