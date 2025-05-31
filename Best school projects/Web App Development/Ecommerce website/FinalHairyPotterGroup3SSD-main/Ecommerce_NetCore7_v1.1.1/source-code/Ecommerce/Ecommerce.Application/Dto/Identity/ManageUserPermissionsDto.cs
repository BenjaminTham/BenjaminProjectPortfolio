namespace Ecommerce.Application.Dto;

public class ManageUserPermissionsDto
{
    public string UserId { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string PermissionValue { get; set; }
    public List<ManageClaimDto> ManagePermissionsDto { get; set; }
    
}
