using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Categories.Queries;

public class GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
}
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCategoriesQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _db.Categories.Include(c => c.Children).ThenInclude(o => o.Children).OrderByDescending(o => o.LastModifiedDate).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<CategoryDto>>(categories);
        return result.AsReadOnly();
    }
}
