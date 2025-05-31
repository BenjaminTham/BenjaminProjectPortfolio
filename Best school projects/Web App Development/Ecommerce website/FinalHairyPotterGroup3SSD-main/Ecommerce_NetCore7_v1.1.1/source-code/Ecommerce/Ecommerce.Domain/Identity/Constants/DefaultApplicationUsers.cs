using Ecommerce.Domain.Identity.Entities;

namespace Ecommerce.Domain.Identity.Constants;
public class DefaultApplicationUsers
{
    public static ApplicationUser GetSuperUser()
    {
        var defaultUser = new ApplicationUser
        {
            Id = "9185e0f3-8611-4fc5-9199-dbbe0cccd837",
            UserName = "superadmin",
            Email = "superadmin@domain.com",
            FirstName = "Super",
            LastName = "Admin",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            IsActive = true
        };
        return defaultUser;
    }
}