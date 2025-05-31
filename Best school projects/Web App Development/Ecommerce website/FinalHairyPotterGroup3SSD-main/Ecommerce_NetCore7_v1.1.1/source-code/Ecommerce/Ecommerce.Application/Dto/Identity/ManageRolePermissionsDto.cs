using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class ManageRolePermissionsDto
{

    [Required]
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public string PermissionValue { get; set; }
    public List<ManageClaimDto> ManagePermissionsDto { get; set; }

}
