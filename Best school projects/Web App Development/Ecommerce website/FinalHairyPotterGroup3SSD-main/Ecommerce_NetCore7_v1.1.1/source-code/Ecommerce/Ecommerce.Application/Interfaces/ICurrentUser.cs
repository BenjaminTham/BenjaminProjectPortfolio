using System.Security.Claims;

namespace Ecommerce.Application.Interfaces;

public interface ICurrentUser
{
    string UserId { get; }
    string UserName { get; }
    string UserFullName { get; }
    IReadOnlyList<string> Roles { get; }
    Task<IList<Claim>> Permissions();
}
