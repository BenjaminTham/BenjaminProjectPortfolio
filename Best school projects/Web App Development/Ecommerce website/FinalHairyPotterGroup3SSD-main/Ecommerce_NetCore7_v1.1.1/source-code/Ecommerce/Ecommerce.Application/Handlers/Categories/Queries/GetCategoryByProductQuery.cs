using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Categories.Queries;

public class GetCategoryByProductQuery : IRequest<CategoryDto>
{
    public int Id { get; set; }
}
public class GetCategoryByProductQueryHandler : IRequestHandler<GetCategoryByProductQuery, CategoryDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCategoryByProductQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByProductQuery request, CancellationToken cancellationToken)
    {
        var category = await _db.Products.Include(o => o.Category).Where(o => o.Id == request.Id).Select(o => o.Category).FirstOrDefaultAsync(); ;
        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }
}
