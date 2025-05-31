using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.ContactQueries.Queries;

public class GetContactQueryByIdQuery : IRequest<ContactQueryDto>
{
    public long Id { get; set; }
}
public class GetContactQueryByIdQueryHandler : IRequestHandler<GetContactQueryByIdQuery, ContactQueryDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetContactQueryByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ContactQueryDto> Handle(GetContactQueryByIdQuery request, CancellationToken cancellationToken)
    {
        var contactQuery = await _db.ContactQueries.FindAsync(request.Id);
        var result = _mapper.Map<ContactQueryDto>(contactQuery);
        return result;
    }
}
