using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Identity.Permissions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.Application.Services;

public class RoleService : IRoleService
{
    private readonly IApplicationRoleManager _roleManager;
    private readonly IMapper _mapper;
    private readonly IPermissionHelper _permissionHelper;

    public RoleService(IApplicationRoleManager roleManager, IMapper mapper, IPermissionHelper permissionHelper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _permissionHelper = permissionHelper;
    }
    public async Task<Response<string>> AddRoleAsync(AddEditRoleDto addRoleDto)
    {
        if (await _roleManager.GetRoleAsync(addRoleDto.Name) != null)
            return Response<string>.Fail("The role already exists. Please try different one!");
        var appRole = _mapper.Map<IdentityRole>(addRoleDto);
        var rs = await _roleManager.AddRoleAsync(appRole);
        return rs.Succeeded
            ? Response<string>.Success(appRole.Id, "New role has been created")
            : Response<string>.Fail("Failed to create new role");
    }

    public async Task<Response<AddEditRoleDto>> GetRoleByIdAsync(string roleId)
    {
        var userRole = await _roleManager.FindByIdAsync(roleId);
        if (userRole == null) return Response<AddEditRoleDto>.Fail("User does not exists");
        var roleDto = _mapper.Map<AddEditRoleDto>(userRole);
        return Response<AddEditRoleDto>.Success(roleDto, "Retrieved successfully");
    }

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        var roles = await _roleManager.GetRolesAsync();
        var result = _mapper.Map<List<RoleDto>>(roles);
        return result.AsReadOnly();
    }

    public async Task<Response<UserIdentityDto>> UpdateRoleAsync(AddEditRoleDto editUserDto)
    {
        var role = await _roleManager.FindByIdAsync(editUserDto.Id);
        _mapper.Map(editUserDto, role);
        var rs = await _roleManager.UpdateAsync(role);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = role.Id }, rs.Message)
            : Response<UserIdentityDto>.Fail(rs.Message);
    }

    public async Task<Response<ManageRolePermissionsDto>> ManagePermissionsAsync(string roleId, string permissionValue)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return Response<ManageRolePermissionsDto>.Fail("No Role Exists");
        var roleClaims = await _roleManager.GetClaimsAsync(role);
        var allPermissions = _permissionHelper.GetAllPermissions();

        if (!string.IsNullOrWhiteSpace(permissionValue))
        {
            allPermissions = allPermissions.Where(x => x.Value.ToLower().Contains(permissionValue.Trim().ToLower())).ToList();
        }
        var managePermissionsClaim = new List<ManageClaimDto>();
        foreach (var permission in allPermissions)
        {
            var managePermissionClaim = new ManageClaimDto { Type = permission.Type, Value = permission.Value };
            if (roleClaims.Any(x => x.Value == permission.Value))
            {
                managePermissionClaim.Checked = true;
            }
            managePermissionsClaim.Add(managePermissionClaim);
        }

        var manageRolePermissionsDto = new ManageRolePermissionsDto
        {
            RoleId = roleId,
            RoleName = role.Name,
            PermissionValue = permissionValue,
            ManagePermissionsDto = managePermissionsClaim
        };
        return allPermissions.Count > 0
            ? Response<ManageRolePermissionsDto>.Success(manageRolePermissionsDto, "Successfully retrieved")
            : Response<ManageRolePermissionsDto>.Fail(
                $"No Permissions exists! Something is Wrong with Permission source file");
    }

    public async Task<Response<RoleIdentityDto>> ManageRoleClaimAsync(ManageRoleClaimDto manageRoleClaimDto)
    {
        var roleById = await _roleManager.FindByIdAsync(manageRoleClaimDto.RoleId);
        var roleByName = await _roleManager.GetRoleAsync(manageRoleClaimDto.RoleName);
        if (roleById != roleByName)
            return Response<RoleIdentityDto>.Fail("Forbidden");
        var allClaims = await _roleManager.GetClaimsAsync(roleById);
        var claimExists =
            allClaims.Where(x => x.Type == manageRoleClaimDto.Type && x.Value == manageRoleClaimDto.Value).ToList();
        switch (manageRoleClaimDto.Checked)
        {
            case true when claimExists.Count == 0:
                await _roleManager.AddClaimAsync(roleById, new Claim(manageRoleClaimDto.Type, manageRoleClaimDto.Value));
                break;
            case false when claimExists.Count > 0:
                {
                    foreach (var claim in claimExists)
                    {
                        await _roleManager.RemoveClaimAsync(roleById, claim);
                    }
                    break;
                }
        }
        return Response<RoleIdentityDto>.Success(new RoleIdentityDto { RoleId = manageRoleClaimDto.RoleId },
            "Succeeded");
    }

    public async Task<Response<UserIdentityDto>> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return Response<UserIdentityDto>.Fail("This Role not exists");
        var rs = await _roleManager.RemoveAsync(role);
        return rs.Succeeded
            ? Response<UserIdentityDto>.Success(new UserIdentityDto { Id = role.Id }, rs.ToString())
            : Response<UserIdentityDto>.Fail(rs.ToString());
    }
}