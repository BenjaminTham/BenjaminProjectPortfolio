using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.OrderStatusValues.Queries;

public class GetOrderStatusValuesQuery : IRequest<IEnumerable<OrderStatusValueDto>>
{
}
public class GetOrderStatusValuesQueryHandler : IRequestHandler<GetOrderStatusValuesQuery, IEnumerable<OrderStatusValueDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderStatusValuesQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderStatusValueDto>> Handle(GetOrderStatusValuesQuery request, CancellationToken cancellationToken)
    {
        var orderStatusValue = await _db.OrderStatusValues
            .OrderByDescending(o => o.LastModifiedDate)
            .ProjectTo<OrderStatusValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return orderStatusValue;
    }
}
