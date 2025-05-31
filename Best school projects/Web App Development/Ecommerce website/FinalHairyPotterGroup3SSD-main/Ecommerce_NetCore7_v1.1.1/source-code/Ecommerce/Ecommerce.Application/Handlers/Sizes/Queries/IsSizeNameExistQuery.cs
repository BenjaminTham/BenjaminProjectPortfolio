using Ecommerce.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace Ecommerce.Application.Handlers.Sizes.Queries;

public class IsSizeNameExistQuery : IRequest<bool>
{
    public required string Name { get; set; }
}
public class IsSizeNameExistQueryHandler : IRequestHandler<IsSizeNameExistQuery, bool>
{
    private readonly IDataContext _db;
    public IsSizeNameExistQueryHandler(IDataContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(IsSizeNameExistQuery request, CancellationToken cancellationToken)
    {
        return await _db.Sizes.AnyAsync(p => p.Name.ToLower() == request.Name.ToLower());
    }
}
