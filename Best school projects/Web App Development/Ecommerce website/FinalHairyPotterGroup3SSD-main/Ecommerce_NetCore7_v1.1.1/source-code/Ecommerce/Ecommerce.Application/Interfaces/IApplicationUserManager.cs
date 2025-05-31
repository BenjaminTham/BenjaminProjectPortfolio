using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;
using System.Security.Claims;

namespace Ecommerce.Application.Interfaces;

public interface IApplicationUserManager
{
    Task<IdentityResponse> RegisterUserAsync(ApplicationUser user);
    Task<ApplicationUser> FindByEmailAsync(string email);
    Task<ApplicationUser> GetUserByNameAsync(string userName);
    Task<ApplicationUser> GetUserByIdAsync(string userId);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
    Task<ApplicationUser> GetUserAsync(ClaimsPrincipal claimsPrincipal);
    Task<IdentityResponse> AddToRoleAsync(ApplicationUser user, string roleName);
    Task<IdentityResponse> AddToRolesAsync(ApplicationUser user, List<string> roleNames);
    Task<IdentityResponse> RemoveFromRoleAsync(ApplicationUser user, string roleName);
    Task<IdentityResponse> RemoveFromRolesAsync(ApplicationUser user, List<string> roleNames);
    Task<IdentityResponse> AddClaimsAsync(ApplicationUser user, List<Claim> claims);
    Task<IdentityResponse> AddClaimAsync(ApplicationUser user, Claim claim);
    Task<IdentityResponse> RemoveClaimsAsync(ApplicationUser user, List<Claim> claims);
    Task<IdentityResponse> UpdateAsync(ApplicationUser user);
    Task<IdentityResponse> HasClaimAsync(ApplicationUser user, Claim claim);
    IQueryable<ApplicationUser> Users();
    Task<IEnumerable<ApplicationUser>> GetUsersAsync();
    Task<IdentityResponse> AddUserAsync(ApplicationUser user, string password);
    Task<IdentityResponse> RemoveAsync(ApplicationUser user);
    Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
    Task<IdentityResponse> ConfirmEmailAsync(ApplicationUser user, string tokenProvider);
    Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);

}
