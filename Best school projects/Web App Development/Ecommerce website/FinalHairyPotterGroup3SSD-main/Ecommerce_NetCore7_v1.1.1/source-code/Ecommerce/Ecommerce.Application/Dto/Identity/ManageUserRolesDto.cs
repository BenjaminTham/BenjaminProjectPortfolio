using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dto;

public class ManageUserRolesDto
{
    [Required]
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string RoleName { get; set; }
    public bool Checked { get; set; }
}
