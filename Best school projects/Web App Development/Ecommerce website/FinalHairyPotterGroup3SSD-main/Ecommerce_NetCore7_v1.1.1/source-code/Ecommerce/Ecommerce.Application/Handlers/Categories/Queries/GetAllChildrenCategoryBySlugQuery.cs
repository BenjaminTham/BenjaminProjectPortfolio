using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Categories.Queries;

public class GetAllChildrenCategoryBySlugQuery : IRequest<CategoryDto>
{
    public string Slug { get; set; }
}
public class GetAllChildrenCategoryBySlugQueryHandler : IRequestHandler<GetAllChildrenCategoryBySlugQuery, CategoryDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetAllChildrenCategoryBySlugQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetAllChildrenCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .Where(o => o.Slug == request.Slug)
            .Include(o => o.Children)
            .ThenInclude(o => o.Children)
            .FirstOrDefaultAsync();
        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }
}
