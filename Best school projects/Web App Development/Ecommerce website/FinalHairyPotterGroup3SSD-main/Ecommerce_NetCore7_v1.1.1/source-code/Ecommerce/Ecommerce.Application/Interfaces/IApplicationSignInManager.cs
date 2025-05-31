using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IApplicationSignInManager
{
    Task<IdentityResponse> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent, bool lockoutOnFailure);
    Task<IdentityResponse> TwoFactorSignInAsync(ApplicationUser user, string provider, string twoFactorCode, bool isPersistent, bool lockoutOnFailure);
    Task<IdentityResponse> ResetPasswordAsync(ApplicationUser user, string token, string password);
    Task SignOutAsync();
}
