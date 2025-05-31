using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetOrderWithItemsByIdQuery : IRequest<List<OrderItemDto>>
{
    public int Id { get; set; }
}
public class GetOrderWithItemsByIdQueryHandler : IRequestHandler<GetOrderWithItemsByIdQuery, List<OrderItemDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderWithItemsByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<OrderItemDto>> Handle(GetOrderWithItemsByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.OrderDetails
            .Where(o => o.OrderId == request.Id)
            .Select(o => new OrderItemDto
            {
                OrderId = o.OrderId,
                Item = o.ProductName,
                Quantity = o.Qty,
                UnitPrice = o.UnitPrice,
            })
            .ToListAsync();
        return order;
    }
}
