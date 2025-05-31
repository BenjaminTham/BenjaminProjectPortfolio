using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.DeliveryMethods.Queries;

public class GetDeliveryMethodsQuery : IRequest<IEnumerable<DeliveryMethodDto>>
{
}
public class GetDeliveryMethodsQueryHandler : IRequestHandler<GetDeliveryMethodsQuery, IEnumerable<DeliveryMethodDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetDeliveryMethodsQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DeliveryMethodDto>> Handle(GetDeliveryMethodsQuery request, CancellationToken cancellationToken)
    {
        var deliveryMethods = await _db.DeliveryMethods
            .OrderByDescending(o => o.LastModifiedDate)
            .ProjectTo<DeliveryMethodDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return deliveryMethods;
    }
}
