using Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Infrastructure.Sql.Extensions;

public static class IdentityExtensions
{
    public static IdentityResponse ToIdentityResponse(this IdentityResult identityResult)
    {
        return identityResult.Succeeded
            ? IdentityResponse.Success(identityResult.ToString())
            : IdentityResponse.Fail(identityResult.ToString());
    }
}
