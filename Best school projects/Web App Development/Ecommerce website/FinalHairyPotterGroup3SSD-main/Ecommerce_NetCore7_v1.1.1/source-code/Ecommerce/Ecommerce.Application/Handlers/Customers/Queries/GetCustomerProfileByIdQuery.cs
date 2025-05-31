using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Queries;

public class GetCustomerProfileByIdQuery : IRequest<CustomerProfileDto>
{
    public long Id { get; set; }
}
public class GetCustomerProfileByIdQueryHandler : IRequestHandler<GetCustomerProfileByIdQuery, CustomerProfileDto>
{
    private readonly IDataContext _db;
    private readonly IUserService _user;
    private readonly IMapper _mapper;
    public GetCustomerProfileByIdQueryHandler(IDataContext db, IUserService user, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
        _user = user;
    }

    public async Task<CustomerProfileDto> Handle(GetCustomerProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Include(c => c.User).Where(c => c.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (customer == null) throw new Exception("Sorry! No Customer Found!");
        var customerDto = _mapper.Map<CustomerDto>(customer);

        var order = await _db.Orders.Where(o => o.CustomerId == customer.Id).ToListAsync(cancellationToken);
        var orderIds = order.Select(o => o.Id);
        var orderStatus = await _db.OrderStatus.Include(o => o.OrderStatusValue).Where(o => orderIds.Contains(o.OrderId)).ToListAsync(cancellationToken);


        var orderDto = _mapper.Map<List<OrderDto>>(order);
        orderDto.ForEach(o =>
        {
            o.CurrentOrderStatus = orderStatus.Where(c => c.OrderId == o.Id)
                .OrderByDescending(o => o.Id)
                .Select(p => p.OrderStatusValue.StatusValue).FirstOrDefault();
        });

        var orderAmount = order.Sum(o => o.OrderAmount);
        var deliveryCharge = order.Sum(o => o.DeliveryCharge);

        var customerProfile = new CustomerProfileDto
        {
            CustomerInfo = customerDto,
            TotalOrder = order.Count(),
            TotalOrderAmount = orderAmount,
            TotalDeliveryCharge = deliveryCharge,
            TotalAmount = orderAmount + deliveryCharge,
            Orders = orderDto,
        };

        return customerProfile;
    }
}
