using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Categories.Queries;

public class GetCategoryBySlugQuery : IRequest<CategoryDto>
{
    public string Slug { get; set; }
}
public class GetCategoryBySlugQueryHandler : IRequestHandler<GetCategoryBySlugQuery, CategoryDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCategoryBySlugQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.Where(o => o.Slug == request.Slug).FirstOrDefaultAsync();
        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }
}
