using Ecommerce.Domain.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Ecommerce.Infrastructure.Sql.Identity;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }
    //protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    //{
    //    var id = await base.GenerateClaimsAsync(user);

    //    var permissionClaims = id.Claims.Where(x => x.Type == CustomClaimTypes.Permission).ToList();
    //    foreach (var claim in permissionClaims)
    //    {
    //        id.RemoveClaim(claim);
    //    }
    //    return id;
    //}

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName + " " + user.LastName ?? ""));
        return identity;
    }
}
