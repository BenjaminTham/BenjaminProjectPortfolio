using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Infrastructure.Sql.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Infrastructure.Sql.Repositories;

public class ApplicationRoleManager : IApplicationRoleManager
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationRoleManager(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IList<Claim>> GetClaimsAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        return await _roleManager.GetClaimsAsync(role);
    }

    public async Task<IList<Claim>> GetClaimsAsync(IList<string> roleNames)
    {
        var roles = _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToList();
        var allClaims = new List<Claim>();

        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            allClaims.AddRange(claims);
        }
        return allClaims;
    }

    public async Task<IdentityRole> GetRoleAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<IdentityRole> FindByIdAsync(string roleId)
    {
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<IList<Claim>> GetClaimsAsync(IdentityRole role)
    {
        return await _roleManager.GetClaimsAsync(role);
    }

    public async Task<IdentityResponse> RemoveClaimAsync(IdentityRole role, Claim claim)
    {
        var rs = await _roleManager.RemoveClaimAsync(role, claim);
        return rs.ToIdentityResponse();
    }

    public async Task<IdentityResponse> AddClaimAsync(IdentityRole role, Claim claim)
    {
        var rs = await _roleManager.AddClaimAsync(role, claim);
        return rs.ToIdentityResponse();
    }

    public IQueryable<IdentityRole> Roles()
    {
        return _roleManager.Roles.Where(x => x.Name != DefaultApplicationRoles.SuperAdmin);
    }

    /// <inheritdoc />
    public async Task<IdentityResponse> AddRoleAsync(IdentityRole applicationRole)
    {
        var rs = await _roleManager.CreateAsync(applicationRole);
        return rs.ToIdentityResponse();
    }

    public async Task<IEnumerable<IdentityRole>> GetRolesAsync()
    {
        return await _roleManager.Roles.Where(x => x.Name != DefaultApplicationRoles.SuperAdmin).ToListAsync();
    }

    public async Task<IdentityResponse> UpdateAsync(IdentityRole role)
    {
        var rs = await _roleManager.UpdateAsync(role);
        return rs.ToIdentityResponse();
    }

    public async Task<IdentityResponse> RemoveAsync(IdentityRole identityRole)
    {
        var rs = await _roleManager.DeleteAsync(identityRole);
        return rs.ToIdentityResponse();
    }
}
