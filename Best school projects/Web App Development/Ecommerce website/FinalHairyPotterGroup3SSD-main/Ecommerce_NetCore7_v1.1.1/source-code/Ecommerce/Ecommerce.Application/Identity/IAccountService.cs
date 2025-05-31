using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Identity;

public interface IAccountService
{
    Task<Response<UserIdentityDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<Response<UserIdentityDto>> SignInAsync(LoginUserDto loginUserDto);
    Task<Response<UserIdentityDto>> TwoFactorSignInAsync(TwoStepDto twoStepDto);
    Task<Response<UserIdentityDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<Response<UserIdentityDto>> ConfirmEmailAsync(string email, string token);
    Task SignOutAsync();
}
