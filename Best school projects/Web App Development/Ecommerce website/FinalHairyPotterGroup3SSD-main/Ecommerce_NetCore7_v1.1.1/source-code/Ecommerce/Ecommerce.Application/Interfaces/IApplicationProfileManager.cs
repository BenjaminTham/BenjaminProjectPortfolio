using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IApplicationProfileManager
{
    Task<IdentityResponse> UpdatePasswordAsync(ApplicationUser user, string oldPassword, string newPasswod);
    Task<IdentityResponse> UpdateProfileAsync(ApplicationUser user);
    Task<bool> GetTwoFactorAuthorizationAsync(ApplicationUser user);
    Task<IdentityResponse> UpdateTwoFactorAuthorizationAsync(ApplicationUser user, bool isEnabled);
}
