using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetCurrentCustomerOrdersQuery : IRequest<List<CustomerOrderDto>>
{
}
public class GetCurrentCustomerOrdersQueryHandler : IRequestHandler<GetCurrentCustomerOrdersQuery, List<CustomerOrderDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    public GetCurrentCustomerOrdersQueryHandler(IDataContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<CustomerOrderDto>> Handle(GetCurrentCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Where(c => c.ApplicationUserId == _currentUser.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if(customer == null) return new List<CustomerOrderDto>();
        var orders = await _db.Orders.AsNoTracking().Include(o => o.OrderDetails)
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
            .Where(o => o.CustomerId == customer.Id)
            .Select(o => new CustomerOrderDto
            {
                OrderId = o.Id,
                InvoiceNo = o.InvoiceNo,
                OrderDate = o.CreatedDate,
                OrderAmount = o.OrderAmount,
                DeliveryCharge = o.DeliveryCharge,
                Total = o.OrderAmount + o.DeliveryCharge,
                OrderStatus = o.OrderStatus.OrderByDescending(o => o.Id).Select(o => o.OrderStatusValue).FirstOrDefault().StatusValue,
                OrderDetails = _mapper.Map<List<OrderDetailsDto>>(o.OrderDetails)
            })
            .OrderByDescending(c=>c.OrderId).ToListAsync(cancellationToken);

        return orders;
    }
}
