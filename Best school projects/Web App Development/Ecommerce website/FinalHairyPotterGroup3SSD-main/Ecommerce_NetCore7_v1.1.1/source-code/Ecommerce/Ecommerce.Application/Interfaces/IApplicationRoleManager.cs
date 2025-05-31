using Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.Application.Interfaces;

public interface IApplicationRoleManager
{
    Task<IList<Claim>> GetClaimsAsync(string roleName);
    Task<IList<Claim>> GetClaimsAsync(IList<string> roleNames);
    Task<IdentityRole> GetRoleAsync(string roleName);
    Task<IdentityRole> FindByIdAsync(string roleId);
    Task<IList<Claim>> GetClaimsAsync(IdentityRole role);
    Task<IdentityResponse> RemoveClaimAsync(IdentityRole role, Claim claim);
    Task<IdentityResponse> AddClaimAsync(IdentityRole role, Claim claim);
    IQueryable<IdentityRole> Roles();
    Task<IEnumerable<IdentityRole>> GetRolesAsync();
    Task<IdentityResponse> UpdateAsync(IdentityRole role);

    /// <summary>
    /// Add new role to database role table.
    /// </summary>
    /// <param name="identityRole"></param>
    /// <returns>IdentityRole object.</returns>
    Task<IdentityResponse> AddRoleAsync(IdentityRole identityRole);
    Task<IdentityResponse> RemoveAsync(IdentityRole identityRole);
}
