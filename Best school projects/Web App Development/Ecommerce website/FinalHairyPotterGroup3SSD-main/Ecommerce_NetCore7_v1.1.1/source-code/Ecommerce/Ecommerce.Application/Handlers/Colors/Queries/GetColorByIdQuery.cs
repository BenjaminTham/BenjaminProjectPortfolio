using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.Colors.Queries;

public class GetColorByIdQuery : IRequest<ColorDto>
{
    public int Id { get; set; }
}
public class GetColorByIdQueryHandler : IRequestHandler<GetColorByIdQuery, ColorDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetColorByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ColorDto> Handle(GetColorByIdQuery request, CancellationToken cancellationToken)
    {
        var color = await _db.Colors.FindAsync(request.Id);
        var result = _mapper.Map<ColorDto>(color);
        return result;
    }
}
