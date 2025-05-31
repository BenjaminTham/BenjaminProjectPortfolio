using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Colors.Queries;

public class GetColorsQuery : IRequest<IEnumerable<ColorDto>>
{
}
public class GetColorsQueryHandler : IRequestHandler<GetColorsQuery, IEnumerable<ColorDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetColorsQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ColorDto>> Handle(GetColorsQuery request, CancellationToken cancellationToken)
    {
        var colors = await _db.Colors.OrderByDescending(o => o.LastModifiedDate).ToListAsync();

        var result = _mapper.Map<List<ColorDto>>(colors);
        return result.AsReadOnly();
    }
}
