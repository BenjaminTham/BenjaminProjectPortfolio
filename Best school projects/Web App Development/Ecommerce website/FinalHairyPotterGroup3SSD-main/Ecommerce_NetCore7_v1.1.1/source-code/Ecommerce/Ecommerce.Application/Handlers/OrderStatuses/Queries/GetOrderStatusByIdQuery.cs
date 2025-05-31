using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.OrderStatuses.Queries;

public class GetOrderStatusByIdQuery : IRequest<IEnumerable<OrderStatusDto>>
{
    public long OrderId { get; set; }
}
public class GetOrderStatusByIdQueryHandler : IRequestHandler<GetOrderStatusByIdQuery, IEnumerable<OrderStatusDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderStatusByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderStatusDto>> Handle(GetOrderStatusByIdQuery request, CancellationToken cancellationToken)
    {
        var orderStatus = await _db.OrderStatus.Include(o => o.OrderStatusValue).Where(o => o.OrderId == request.OrderId).OrderBy(o => o.Id).ToListAsync();
        var result = _mapper.Map<List<OrderStatusDto>>(orderStatus);
        return result;
    }
}
