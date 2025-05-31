using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using Ecommerce.Domain.Identity.Permissions;
using Ecommerce.Web.Mvc.Extension;
using Ecommerce.Web.Mvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Web.Mvc.Areas.Manager.Controllers;

[Authorize]
public class RoleController : Controller
{
    private readonly IRoleService _roleService;
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Authorize(Permissions.Permissions_Role_View)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Permissions.Permissions_Role_View)]
    [HttpPost]
    public async Task<IActionResult> GetRoles()
    {
        var paging = new PageRequest().PostPageResponse(Request);

        var response = await _roleService.GetRolesAsync();
        var recordsFilteredCount = response.Count(a => a.Name.ToLower().Contains(paging.SearchValue));
        var recordsTotalCount = response.Count();

        response = response.AsQueryable()
                .Where(a => a.Name.ToLower().Contains(paging.SearchValue))
                .OrderBy($"{paging.SortColumnName} {paging.SortOrder}")
                .Skip(paging.Start).Take(paging.Length)
                .ToList();


        var jsonData = new { data = response, draw = paging.Draw, recordsFiltered = recordsFilteredCount, recordsTotal = recordsTotalCount };
        return Json(jsonData);
    }

    [HttpGet("[controller]/[action]/{id}")]
    [Authorize(Permissions.Permissions_Role_Edit)]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var response = await _roleService.GetRoleByIdAsync(id);
        return Json(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_Role_Create)]
    public async Task<IActionResult> CreateRole(AddEditRoleDto addRoleDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _roleService.AddRoleAsync(addRoleDto);
            if (response.Succeeded) return Json(new JsonResponse { Success = true, Data = response });
        }
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Permissions.Permissions_Role_Edit)]
    public async Task<IActionResult> EditRole(AddEditRoleDto editRoleDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _roleService.UpdateRoleAsync(editRoleDto);
            if (response.Succeeded) return Json(new JsonResponse { Success = true, Data = response });
        }
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }

    [HttpGet]
    [Authorize(Permissions.Permissions_RolePermission_Edit)]
    public async Task<IActionResult> GetPermissionsByRole(string id, string permissionValue)
    {
        var response = await _roleService.ManagePermissionsAsync(id, permissionValue);
        if (!response.Succeeded)
            return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
        return Json(response.Data);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_RolePermission_Edit)]
    public async Task<IActionResult> UpdateRolePermission(ManageRoleClaimDto manageRoleClaimDto)
    {
        if (!ModelState.IsValid)
            return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
        var response = await _roleService.ManageRoleClaimAsync(manageRoleClaimDto);
        return Json(new JsonResponse { Success = true, Data = response });
    }

    [Authorize(Permissions.Permissions_User_Delete)]
    [HttpPost]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var rs = await _roleService.DeleteRoleAsync(id);
        if (rs.Succeeded) return Json(rs);
        return Json(new JsonResponse { Success = false, Error = ModelState.ToSerializedDictionary() });
    }
}
