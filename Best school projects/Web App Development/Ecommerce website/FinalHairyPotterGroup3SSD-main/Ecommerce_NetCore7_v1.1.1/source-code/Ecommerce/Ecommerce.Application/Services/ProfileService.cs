using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IApplicationProfileManager _profileManager;
    private readonly IMediaService _mediaService;
    private readonly IApplicationUserManager _userManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public ProfileService(IApplicationProfileManager profileManager,
                        IMediaService mediaService,
                        IApplicationUserManager userManager,
                        IMapper mapper,
                        ICurrentUser currentUser)
    {
        _profileManager = profileManager;
        _mediaService = mediaService;
        _userManager = userManager;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Response<EditProfileDto>> GetCurrentUserAsync()
    {
        var appUser = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        if (appUser == null) return Response<EditProfileDto>.Fail("User does not exists");
        var userImage = await _mediaService.GetByIdAsync(appUser.ProfilePicture);

        var userDto = _mapper.Map<EditProfileDto>(appUser);
        userDto.UserFromDate = appUser.CreatedDate;
        userDto.ProfilePicturePreview = userImage?.Name;
        return Response<EditProfileDto>.Success(userDto, "Retrieved successfully");
    }

    public async Task<Response<TwoStepDto>> GetTwoFactorAuthorizationAsync()
    {
        var response = new TwoStepDto();
        var appUser = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        var rs = await _profileManager.GetTwoFactorAuthorizationAsync(appUser);
        response.IsEmailConfirmed = appUser.EmailConfirmed;
        response.IsEnabled = rs;
        return Response<TwoStepDto>.Success(response, "Retrieved successfully");
    }

    public async Task<Response<UserIdentityDto>> UpdateTwoFactorAuthorizationAsync(TwoStepDto twoStepDto)
    {
        var appUser = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        var rs = await _profileManager.UpdateTwoFactorAuthorizationAsync(appUser, twoStepDto.IsEnabled);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }

    public async Task<Response<UserIdentityDto>> UpdatePasswordAsync(EditPasswordDto editPasswordDto)
    {
        var user = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        var rs = await _profileManager.UpdatePasswordAsync(user, editPasswordDto.OldPassword, editPasswordDto.NewPassword);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }

    public async Task<Response<UserIdentityDto>> UpdateProfileAsync(EditProfileDto editProfileDto)
    {
        var user = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        if (user == null) return Response<UserIdentityDto>.Fail("No user exists by this username");

        var userByEmail = await _userManager.FindByEmailAsync(editProfileDto.Email);
        if (userByEmail != null && userByEmail.Id != user.Id) return Response<UserIdentityDto>.Fail("The email address is not available");

        user.FirstName = editProfileDto.FirstName;
        user.LastName = editProfileDto.LastName;
        user.ProfilePicture = editProfileDto.ProfilePicture;
        user.Email = editProfileDto.Email;
        user.PhoneNumber = editProfileDto.PhoneNumber;
        user.DateOfBirth = editProfileDto.DateOfBirth;
        user.Gender = editProfileDto.Gender;
        user.LastModifiedDate = DateTime.UtcNow;
        user.LastModifiedBy = _currentUser.UserName;
        var rs = await _profileManager.UpdateProfileAsync(user);

        return rs.Succeeded
        ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.ToString())
        : Response<UserIdentityDto>.Fail(rs.ToString());

    }


}
