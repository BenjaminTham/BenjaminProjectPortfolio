using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Extension;
using Ecommerce.Web.Mvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IKeyAccessor _keyAccessor;
    public UserController(IUserService userService, IKeyAccessor keyAccessor)
    {
        _userService = userService;
        _keyAccessor = keyAccessor;
    }
    [Authorize(Permissions.Permissions_User_View)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Permissions.Permissions_User_View)]
    [HttpPost]
    public async Task<IActionResult> GetUsers()
    {
        var paging = new PageRequest().PostPageResponse(Request);

        var response = await _userService.GetUsersAsync();
        int recordsFilteredCount =
                response.AsQueryable()
                .Where(a => a.UserName.ToLower().Contains(paging.SearchValue))
                .Count();
        // Total Records Count
        int recordsTotalCount = response.Count();

        response =
                response.AsQueryable()
                .Where(a => a.UserName.ToLower().Contains(paging.SearchValue))
                .OrderBy($"{paging.SortColumnName} {paging.SortOrder}")
                .Skip(paging.Start)
                .Take(paging.Length)
                .ToList();

        var jsonData = new { data = response, draw = paging.Draw, recordsFiltered = recordsFilteredCount, recordsTotal = recordsTotalCount };
        return Json(jsonData);
    }

    [HttpGet("[controller]/[action]/{id}")]
    [Authorize(Permissions.Permissions_User_Edit)]
    public async Task<IActionResult> GetUserById(string id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        return Json(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_User_Create)]
    public async Task<IActionResult> CreateUser(AddEditUserDto addUserDto)
    {
        SecurityConfigurationDto conSec = _keyAccessor?["SecurityConfiguration"] != null ? JsonSerializer.Deserialize<SecurityConfigurationDto>(_keyAccessor["SecurityConfiguration"])! : new SecurityConfigurationDto();
        addUserDto.Password = addUserDto.IsDefaultPassword == true ? "P@ssword123" : addUserDto.Password;
        addUserDto.ConfirmPassword = addUserDto.IsDefaultPassword == true ? "P@ssword123" : addUserDto.ConfirmPassword;

        if (addUserDto?.Password?.Length < conSec.PasswordRequiredLength)
        {
            ModelState.AddModelError(addUserDto.Password, $"The Password must be at least {conSec.PasswordRequiredLength} characters long.");
        }

        if (ModelState.IsValid)
        {
            var response = await _userService.AddUserAsync(addUserDto);
            if (response.Succeeded) return Json(response);
            ModelState.AddModelError(string.Empty, response.Message);
        }
        return Json(new JsonResponse { Success = false, Error = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_User_Edit)]
    public async Task<IActionResult> UpdateUser(AddEditUserDto addUserDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _userService.UpdateUserAsync(addUserDto);
            if (response.Succeeded) return Json(response);
            ModelState.AddModelError(string.Empty, response.Message);
        }
        return Json(new JsonResponse { Success = false,
            Error = ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray() });
    }

    [HttpGet("[controller]/[action]/{id}")]
    [Authorize(Permissions.Permissions_UserRole_Edit)]
    public async Task<IActionResult> GetUserRolesById(string id)
    {
        var response = await _userService.ManageRolesAsync(id);
        if (!response.Succeeded)
            return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
        return Json(response.Data);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_UserRole_Edit)]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesDto manageUserRolesDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _userService.ManageRolesAsync(manageUserRolesDto);
            if (response.Succeeded)
                return Json(new JsonResponse { Success = true, Data = response.Data.Id });
            ModelState.AddModelError(string.Empty, response.Message);
        }
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }

    [HttpGet("[controller]/[action]/{id}")]
    [Authorize(Permissions.Permissions_UserPermission_Edit)]
    public async Task<IActionResult> GetPermissionsByUser(string id, string permissionValue)
    {
        var response = await _userService.ManagePermissionsAsync(id, permissionValue);
        if (!response.Succeeded)
            return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
        return Json(response.Data);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_UserPermission_Edit)]
    public async Task<IActionResult> UpdateUserPermission(ManageUserClaimDto manageUserClaimDto)
    {
        if (!ModelState.IsValid)
            return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
        var response = await _userService.ManageUserClaimAsync(manageUserClaimDto);
        return Json(new JsonResponse { Success = true, Data = response });
    }

    [Authorize(Permissions.Permissions_User_Delete)]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var rs = await _userService.DeleteUserAsync(id);
        if (rs.Succeeded) return Json(rs);
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }
}
