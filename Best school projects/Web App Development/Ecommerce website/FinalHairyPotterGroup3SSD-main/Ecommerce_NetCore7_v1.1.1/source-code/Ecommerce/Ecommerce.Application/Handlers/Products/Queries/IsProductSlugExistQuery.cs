using Ecommerce.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace Ecommerce.Application.Handlers.Products.Queries;


public class IsProductSlugExistQuery : IRequest<bool>
{
    public required string Slug { get; set; }
}
public class IsProductSlugExistQueryHandler : IRequestHandler<IsProductSlugExistQuery, bool>
{
    private readonly IDataContext _db;
    public IsProductSlugExistQueryHandler(IDataContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(IsProductSlugExistQuery request, CancellationToken cancellationToken)
    {
        return await _db.Products.AnyAsync(p => p.Slug.ToLower() == request.Slug.ToLower());
    }
}
