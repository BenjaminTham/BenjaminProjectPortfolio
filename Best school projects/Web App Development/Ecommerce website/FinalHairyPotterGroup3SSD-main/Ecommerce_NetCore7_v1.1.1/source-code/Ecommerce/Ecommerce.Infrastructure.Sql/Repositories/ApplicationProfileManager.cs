using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;
using Ecommerce.Infrastructure.Sql.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Infrastructure.Sql.Repositories;

public class ApplicationProfileManager : IApplicationProfileManager
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationProfileManager(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> GetTwoFactorAuthorizationAsync(ApplicationUser user)
    {
        var rs = await _userManager.GetTwoFactorEnabledAsync(user);
        return rs;
    }
    public async Task<IdentityResponse> UpdateTwoFactorAuthorizationAsync(ApplicationUser user, bool IsEnabled)
    {
        var rs = await _userManager.SetTwoFactorEnabledAsync(user, IsEnabled);
        return rs.ToIdentityResponse();
    }


    public async Task<IdentityResponse> UpdatePasswordAsync(ApplicationUser user, string oldPassword, string newPasswod)
    {
        IdentityResult rs = await _userManager.ChangePasswordAsync(user, oldPassword, newPasswod);
        return rs.ToIdentityResponse();
    }

    public async Task<IdentityResponse> UpdateProfileAsync(ApplicationUser user)
    {
        //var userbyname = await _userManager.FindByNameAsync(user.UserName);

        if (user != null)
        {
            var rs = await _userManager.UpdateAsync(user);
            return rs.ToIdentityResponse();
        }
        return null;

    }


}
