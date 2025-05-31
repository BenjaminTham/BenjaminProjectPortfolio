using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.DeliveryMethods.Queries;

public class GetDeliveryMethodByIdQuery : IRequest<DeliveryMethodDto>
{
    public int Id { get; set; }
}
public class GetDeliveryMethodByIdQueryHandler : IRequestHandler<GetDeliveryMethodByIdQuery, DeliveryMethodDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetDeliveryMethodByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<DeliveryMethodDto> Handle(GetDeliveryMethodByIdQuery request, CancellationToken cancellationToken)
    {
        var deliveryMethod = await _db.DeliveryMethods.FindAsync(request.Id);
        var result = _mapper.Map<DeliveryMethodDto>(deliveryMethod);
        return result;
    }
}
