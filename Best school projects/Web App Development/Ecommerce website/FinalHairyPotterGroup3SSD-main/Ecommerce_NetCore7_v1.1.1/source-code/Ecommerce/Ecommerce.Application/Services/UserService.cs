using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Customers.Commands;
using Ecommerce.Application.Handlers.Customers.Queries;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Domain.Identity.Entities;
using Ecommerce.Domain.Identity.Permissions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Ecommerce.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationUserManager _userManager;
    private readonly IMediaService _mediaService;
    private readonly IApplicationRoleManager _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionHelper _permissionHelper;
    private readonly IKeyAccessor _keyAccessor;
    private readonly IDataContext _db;

    public UserService(IApplicationUserManager userManager,
                        IMediaService mediaService,
                        IApplicationRoleManager roleManager,
                        IMapper mapper,
                        IMediator mediator,
                        ICurrentUser currentUser,
                        IPermissionHelper permissionHelper,
                        IKeyAccessor keyAccessor, IDataContext db, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _mediaService = mediaService;
        _roleManager = roleManager;
        _mapper = mapper;
        _mediator = mediator;
        _currentUser = currentUser;
        _permissionHelper = permissionHelper;
        _keyAccessor = keyAccessor;
        _db = db;
        _signInManager = signInManager;
    }
    public async Task<Response<UserIdentityDto>> AddUserAsync(AddEditUserDto addEditUserDto)
    {
        var timeNow = DateTime.UtcNow;
        var conAdv = _keyAccessor?["AdvancedConfiguration"] != null ? JsonSerializer.Deserialize<AdvancedConfigurationDto>(_keyAccessor["AdvancedConfiguration"]) : new AdvancedConfigurationDto();
        var user = _mapper.Map<ApplicationUser>(addEditUserDto);
        user.CreatedBy = _currentUser.UserName;
        user.CreatedDate = timeNow;
        user.LastModifiedBy = _currentUser.UserName;
        user.LastModifiedDate = timeNow;
        var rs = await _userManager.AddUserAsync(user, addEditUserDto.Password);
        if (rs.Succeeded)
        {
            if (conAdv?.RoleName != null)
            {
                var currentUser = await _userManager.GetUserByNameAsync(user.UserName);
                var roleresult = await _userManager.AddToRoleAsync(currentUser, conAdv.RoleName);
            }
            return Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.ToString());
        }
        return Response<UserIdentityDto>.Fail(rs.ToString());
    }

    public async Task<Response<UserIdentityDto>> DeleteUserAsync(string userId)
    {
        var user = await _userManager.GetUserByIdAsync(userId);
        if (user == null) return Response<UserIdentityDto>.Fail("No User Exists");
        var rs = await _userManager.RemoveAsync(user);

        if (rs.Succeeded)
        {
            var customer = await _mediator.Send(new GetCustomerByUserIdQuery() { Id = user.Id });
            if (customer != null)
            {
                var isDeleted = _mediator.Send(new DeleteCustomerCommand() { Id = customer.Id });
            }
        }

        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }

    public async Task<Response<IList<Claim>>> GetAllClaims(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = await _roleManager.GetClaimsAsync(roles);

        var claims = userClaims.Union(roleClaims).ToList();
        return claims.Count > 0
            ? Response<IList<Claim>>.Success(claims, "Successfully retrieved")
            : Response<IList<Claim>>.Fail("No Claims found");
    }

    public async Task<UserDto> GetCurrentUsersAsync()
    {
        var appUser = await _userManager.GetUserByIdAsync(_currentUser.UserId);
        var result = _mapper.Map<UserDto>(appUser);
        return result;
    }

    public async Task<Response<IList<string>>> GetRolesAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Count > 0
            ? Response<IList<string>>.Success(roles, "Successfully retrieved")
            : Response<IList<string>>.Fail("No Roles found");
    }

    public async Task<Response<AddEditUserDto>> GetUserByEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return Response<AddEditUserDto>.Fail("User does not exists");
        var userDto = _mapper.Map<AddEditUserDto>(appUser);
        return Response<AddEditUserDto>.Success(userDto, "Retrieved successfully");
    }

    public async Task<Response<AddEditUserDto>> GetUserByUserNameAsync(string username)
    {
        var appUser = await _userManager.GetUserByNameAsync(username);
        if (appUser == null) return Response<AddEditUserDto>.Fail("User does not exists");
        var userDto = _mapper.Map<AddEditUserDto>(appUser);
        return Response<AddEditUserDto>.Success(userDto, "Retrieved successfully");
    }

    public async Task<Response<AddEditUserDto>> GetUserByIdAsync(string userId)
    {
        var appUser = await _userManager.GetUserByIdAsync(userId);
        if (appUser == null) return Response<AddEditUserDto>.Fail("User does not exists");
        var userImage = await _mediaService.GetByIdAsync(appUser.ProfilePicture);

        var userDto = _mapper.Map<AddEditUserDto>(appUser);
        userDto.ProfilePicturePreview = userImage != null ? userImage.Name : null;
        return Response<AddEditUserDto>.Success(userDto, "Retrieved successfully");
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _userManager.GetUsersAsync();
        var result = _mapper.Map<List<UserDto>>(users);
        return result.AsReadOnly();
    }

    public async Task<Response<ManageUserPermissionsDto>> ManagePermissionsAsync(string userId, string permissionValue)
    {
        var user = await _userManager.GetUserByIdAsync(userId);
        if (user == null) return Response<ManageUserPermissionsDto>.Fail("No User Exists");
        var userPermissions = await _userManager.GetClaimsAsync(user);
        var allPermissions = _permissionHelper.GetAllPermissions();
        if (!string.IsNullOrWhiteSpace(permissionValue))
        {
            allPermissions = allPermissions.Where(x => x.Value.ToLower().Contains(permissionValue.Trim().ToLower()))
                .ToList();
        }
        var managePermissionsClaim = new List<ManageClaimDto>();
        foreach (var permission in allPermissions)
        {
            var managePermissionClaim = new ManageClaimDto { Type = permission.Type, Value = permission.Value };
            if (userPermissions.Any(x => x.Value == permission.Value))
            {
                managePermissionClaim.Checked = true;
            }
            managePermissionsClaim.Add(managePermissionClaim);
        }

        var manageUserPermissionsDto = new ManageUserPermissionsDto
        {
            UserId = userId,
            UserName = user.UserName,
            FullName = user.FullName,
            PermissionValue = permissionValue,
            ManagePermissionsDto = managePermissionsClaim
        };
        return allPermissions.Count > 0
            ? Response<ManageUserPermissionsDto>.Success(manageUserPermissionsDto, "Successfully retrieved")
            : Response<ManageUserPermissionsDto>.Fail(
                $"No Permissions exist! Something is Wrong with Permission source file");
    }

    public async Task<Response<UserRolesDto>> ManageRolesAsync(string userId)
    {
        var user = await _userManager.GetUserByIdAsync(userId);
        if (user == null) return Response<UserRolesDto>.Fail("No User Exists");
        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles().ToListAsync();
        var allRolesDto = _mapper.Map<List<ManageRoleDto>>(allRoles);
        foreach (var roleDto in allRolesDto.Where(roleDto => userRoles.Contains(roleDto.Name)))
        {
            roleDto.Checked = true;
        }

        var manageUserRolesDto = new UserRolesDto
        {
            UserId = userId,
            UserName = user.UserName,
            ManageRolesDto = allRolesDto
        };
        return allRolesDto.Count > 0
            ? Response<UserRolesDto>.Success(manageUserRolesDto, "Success")
            : Response<UserRolesDto>.Fail("No Roles Found");
    }

    public async Task<Response<UserIdentityDto>> ManageRolesAsync(ManageUserRolesDto manageUserRolesDto)
    {
        var user = await _userManager.GetUserByIdAsync(manageUserRolesDto.UserId);
        if (user == null)
            return Response<UserIdentityDto>.Fail("No User Exists This This Id");
        var existingRoles = await _userManager.GetRolesAsync(user);


        var roleExists = existingRoles.FirstOrDefault(x => x == manageUserRolesDto.RoleName);
        switch (manageUserRolesDto.Checked)
        {
            case true when roleExists == null:
                await _userManager.AddToRoleAsync(user, manageUserRolesDto.RoleName);
                break;
            case false when roleExists != null:
                await _userManager.RemoveFromRoleAsync(user, manageUserRolesDto.RoleName);
                break;
        }
        return Response<UserIdentityDto>.Success(new UserIdentityDto { Id = manageUserRolesDto.UserId }, "Succeeded");
    }

    public async Task<Response<UserIdentityDto>> ManageUserClaimAsync(ManageUserClaimDto manageUserClaimDto)
    {
        var userById = await _userManager.GetUserByIdAsync(manageUserClaimDto.UserId);
        var userByName = await _userManager.GetUserByNameAsync(manageUserClaimDto.UserName);

        if (userById != userByName)
            return Response<UserIdentityDto>.Fail("Forbidden");
        var allClaims = await _userManager.GetClaimsAsync(userById);
        var claimExists =
            allClaims.Where(x => x.Type == manageUserClaimDto.Type && x.Value == manageUserClaimDto.Value).ToList();
        switch (manageUserClaimDto.Checked)
        {
            case true when claimExists.Count == 0:
                {
                    await _userManager.AddClaimAsync(userById,
                        new Claim(manageUserClaimDto.Type, manageUserClaimDto.Value));
                    break;
                }
            case false when claimExists.Count > 0:
                {
                    await _userManager.RemoveClaimsAsync(userById, claimExists);
                    break;
                }
        }
        return Response<UserIdentityDto>.Success(new UserIdentityDto { Id = manageUserClaimDto.UserId },
            "Succeeded");
    }

    public async Task<Response<UserIdentityDto>> UpdateUserAsync(AddEditUserDto addEditUserDto)
    {
        var user = await _userManager.GetUserByIdAsync(addEditUserDto.Id);
        if (user == null) return Response<UserIdentityDto>.Fail("Invalid User");
        user.LastModifiedBy = _currentUser.UserName;
        user.LastModifiedDate = DateTime.UtcNow;

        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains(DefaultApplicationRoles.SuperAdmin.ToString()) && _currentUser.UserId != addEditUserDto.Id)
        {
            return Response<UserIdentityDto>.Fail("You Are Not Authorized to Manipulate this User");
        }

        var userByEmail = await _userManager.FindByEmailAsync(user.Email);
        if (userByEmail != null && userByEmail.Id != addEditUserDto.Id)
            return Response<UserIdentityDto>.Fail("The Email Address is Not Available");
        var userByName = await _userManager.GetUserByNameAsync(addEditUserDto.UserName);
        if (userByName != null && userByName.Id != user.Id)
            return Response<UserIdentityDto>.Fail("The Username is Not Available");


        _mapper.Map(addEditUserDto, user);

        #region Update Claims
        var claims = await _userManager.GetClaimsAsync(user);
        if (claims.Any())
        {
            claims.Where(c => c.Type == "FullName" || c.Type == "ProfilePic");
            await _userManager.RemoveClaimsAsync(user, claims.ToList());
        }
        var profilePic = await _db.Galleries.Where(c => c.Id == user.ProfilePicture).FirstOrDefaultAsync();
        var fullName = new Claim("FullName", user.FullName.ToString());
        if (profilePic?.Name != null)
        {
            await _userManager.AddClaimAsync(user, new Claim("ProfilePic", profilePic.Name.ToString()));
        }
        await _userManager.AddClaimAsync(user, fullName);
        #endregion

        var rs = await _userManager.UpdateAsync(user);
        return (rs.Succeeded)
            ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, rs.Message)
            : Response<UserIdentityDto>.Fail(rs.Message);
    }
}
