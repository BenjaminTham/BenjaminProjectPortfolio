using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetOrderByCustomerIdQuery : IRequest<OrderDto>
{
    public int CustomerId { get; set; }
}
public class GetOrderByCustomerIdQueryHandler : IRequestHandler<GetOrderByCustomerIdQuery, OrderDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderByCustomerIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.Where(o => o.Id == request.CustomerId).FirstOrDefaultAsync(cancellationToken);
        var result = _mapper.Map<OrderDto>(order);
        return result;
    }
}
