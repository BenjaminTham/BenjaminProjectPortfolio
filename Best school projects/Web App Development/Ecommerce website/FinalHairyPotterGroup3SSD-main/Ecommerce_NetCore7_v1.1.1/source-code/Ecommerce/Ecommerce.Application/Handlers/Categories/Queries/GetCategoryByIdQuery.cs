using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.Categories.Queries;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public int Id { get; set; }
}
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCategoryByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FindAsync(request.Id);
        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }
}
