using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Sizes.Queries;

public class GetSizesQuery : IRequest<IEnumerable<SizeDto>>
{
}
public class GetSizesQueryHandler : IRequestHandler<GetSizesQuery, IEnumerable<SizeDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetSizesQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SizeDto>> Handle(GetSizesQuery request, CancellationToken cancellationToken)
    {
        var sizes = await _db.Sizes.OrderByDescending(o => o.LastModifiedDate).ToListAsync();

        var result = _mapper.Map<List<SizeDto>>(sizes);
        return result.AsReadOnly();
    }
}
