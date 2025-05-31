using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Categories.Queries;

public class GetAllParentCategoryBySlugQuery : IRequest<CategoryDto>
{
    public string Slug { get; set; }
}
public class GetAllParentCategoryBySlugQueryHandler : IRequestHandler<GetAllParentCategoryBySlugQuery, CategoryDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetAllParentCategoryBySlugQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetAllParentCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .Include(o => o.ParentCategory)
            .ThenInclude(o => o.ParentCategory.ParentCategory)
            .Where(o => o.Slug == request.Slug).FirstOrDefaultAsync();

        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }
}
