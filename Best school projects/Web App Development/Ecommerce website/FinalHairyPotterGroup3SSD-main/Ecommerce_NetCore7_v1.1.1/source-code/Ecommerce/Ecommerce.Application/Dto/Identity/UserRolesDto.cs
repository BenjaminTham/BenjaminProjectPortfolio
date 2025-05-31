using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class UserRolesDto
{
    [Required]
    public string UserId { get; set; }
    public string UserName { get; set; }
    public IList<ManageRoleDto> ManageRolesDto { get; set; }
}
