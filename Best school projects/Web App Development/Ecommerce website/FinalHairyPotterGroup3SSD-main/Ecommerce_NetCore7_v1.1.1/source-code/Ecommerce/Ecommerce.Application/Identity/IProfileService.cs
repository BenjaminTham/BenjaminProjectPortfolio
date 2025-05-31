using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Entities;

namespace Ecommerce.Application.Identity;

public interface IProfileService
{
    Task<Response<UserIdentityDto>> UpdatePasswordAsync(EditPasswordDto editPasswordDto);
    Task<Response<UserIdentityDto>> UpdateProfileAsync(EditProfileDto editProfileDto);

    Task<Response<EditProfileDto>> GetCurrentUserAsync();
    Task<Response<TwoStepDto>> GetTwoFactorAuthorizationAsync();
    Task<Response<UserIdentityDto>> UpdateTwoFactorAuthorizationAsync(TwoStepDto twoStepDto);
}
