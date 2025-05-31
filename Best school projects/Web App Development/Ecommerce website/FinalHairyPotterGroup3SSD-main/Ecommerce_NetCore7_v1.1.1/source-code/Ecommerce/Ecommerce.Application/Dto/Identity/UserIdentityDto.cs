namespace Ecommerce.Application.Dto;

public class UserIdentityDto
{
    public string Id { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
}


public class UserLoginDto
{
    public string Id { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
}