using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Identity.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Infrastructure.Sql.Identity.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{

    private readonly ICurrentUser _currentUser;

    public PermissionAuthorizationHandler(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }
    protected override async Task HandleRequirementAsync
        (AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User == null || !context.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        var permissions = await _currentUser.Permissions();
        if (permissions == null || permissions.Count == 0)
        {
            context.Fail();
            return;
        }

        if (permissions.Any(x => x.Type == CustomClaimTypes.Permission
                                 && x.Value == requirement.Permission
                                 && x.Issuer == "LOCAL AUTHORITY"))
        {
            context.Succeed(requirement);
            return;
        }
        context.Fail();
    }
}
