using Ecommerce.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace Ecommerce.Application.Handlers.Colors.Queries;


public class IsColorNameOrHexExistQuery : IRequest<bool>
{
    public required string Name { get; set; }
    public required string HexCode { get; set; }
}
public class IsColorNameOrHexExistQueryHandler : IRequestHandler<IsColorNameOrHexExistQuery, bool>
{
    private readonly IDataContext _db;
    public IsColorNameOrHexExistQueryHandler(IDataContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(IsColorNameOrHexExistQuery request, CancellationToken cancellationToken)
    {
        return await _db.Colors.AnyAsync(p => p.Name.ToLower() == request.Name.ToLower() || p.HexCode.ToLower() == request.HexCode.ToLower());
    }
}
