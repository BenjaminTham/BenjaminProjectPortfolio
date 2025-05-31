using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Infrastructure.Sql.Repositories;

public class ApplicationSignInManager : IApplicationSignInManager
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public ApplicationSignInManager(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<IdentityResponse> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var rs = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        return rs.Succeeded ? IdentityResponse.Success(rs.ToString(), rs) : IdentityResponse.Fail(rs.ToString(), rs);
    }

    public async Task<IdentityResponse> TwoFactorSignInAsync(ApplicationUser user, string provider, string twoFactorCode, bool isPersistent, bool lockoutOnFailure)
    {
        //var isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", twoFactorCode);
        //if (!isTokenValid)
        //    return IdentityResponse.Fail(isTokenValid.ToString(), isTokenValid);

        var rs = await _signInManager.TwoFactorSignInAsync(provider, twoFactorCode, isPersistent, lockoutOnFailure);
        return rs.Succeeded ? IdentityResponse.Success(rs.ToString(), rs) : IdentityResponse.Fail(rs.ToString(), rs);
    }

    public async Task<IdentityResponse> ResetPasswordAsync(ApplicationUser user, string token, string password)
    {
        var rs = await _userManager.ResetPasswordAsync(user, token, password);
        return rs.Succeeded ? IdentityResponse.Success(rs.ToString(), rs) : IdentityResponse.Fail(rs.ToString(), rs);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();

        //var user = await _userManager.FindByNameAsync(_currentUser.UserName);
        //var claims = await _userManager.GetClaimsAsync(user);
        //await _userManager.RemoveClaimsAsync(user, claims.ToList());
    }
}
