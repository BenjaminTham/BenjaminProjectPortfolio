using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.Sizes.Queries;

public class GetSizeByIdQuery : IRequest<SizeDto>
{
    public int Id { get; set; }
}
public class GetSizeByIdQueryHandler : IRequestHandler<GetSizeByIdQuery, SizeDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetSizeByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<SizeDto> Handle(GetSizeByIdQuery request, CancellationToken cancellationToken)
    {
        var size = await _db.Sizes.FindAsync(request.Id);
        var result = _mapper.Map<SizeDto>(size);
        return result;
    }
}
