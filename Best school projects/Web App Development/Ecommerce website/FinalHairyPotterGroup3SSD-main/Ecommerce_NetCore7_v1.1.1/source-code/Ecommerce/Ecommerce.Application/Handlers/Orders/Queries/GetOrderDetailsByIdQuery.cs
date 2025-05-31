using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetOrderDetailsByIdQuery : IRequest<OrderInfoDetailsDto>
{
    public long OrderId { get; set; }
}
public class GetOrderDetailsByIdQueryHandler : IRequestHandler<GetOrderDetailsByIdQuery, OrderInfoDetailsDto>
{
    private readonly IDataContext _db;
    private readonly IUserService _user;
    private readonly IMapper _mapper;
    public GetOrderDetailsByIdQueryHandler(IDataContext db, IUserService user, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
        _user = user;
    }

    public async Task<OrderInfoDetailsDto> Handle(GetOrderDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var currentOrderStatus = await _db.OrderStatus.AsNoTracking()
            .Include(o => o.OrderStatusValue)
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync(e => e.OrderId == request.OrderId, cancellationToken);

        var orderById = await _db.Orders.AsNoTracking().Include(o => o.OrderDetails)
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
            .Include(o => o.DeliveryMethod)
            .Include(o => o.OrderPayments)
            .FirstOrDefaultAsync(e => e.Id == request.OrderId, cancellationToken);

        var orderItemDetails = await (from od in _db.OrderDetails
                                      join pv in _db.Variants on od.ProductVariantId equals pv.Id into pvlist
                                      from pv in pvlist.DefaultIfEmpty()
                                      join p in _db.Products on pv.ProductId equals p.Id into plist
                                      from p in plist.DefaultIfEmpty()
                                      join pvi in _db.VariantImages on pv.Id equals pvi.VariantId into pvilist
                                      from pvi in pvilist.DefaultIfEmpty()
                                      join i in _db.Galleries on pvi.ImageId equals i.Id into ilist
                                      from i in ilist.DefaultIfEmpty()
                                      where od.OrderId == request.OrderId
                                      select new OrderItemDetailsDto
                                      {
                                          ProductId = pv.ProductId,
                                          ProductSlug = p.Slug,
                                          ProductVariantId = pv.Id,
                                          OrderItemTitle = od.ProductName,
                                          OrderItemImage = i.Name,
                                          ItemUnitPrice = od.UnitPrice,
                                          ItemQty = od.Qty,
                                      }).ToListAsync(cancellationToken);

        UpdateOrderStatusDto updateOrderStatus = new UpdateOrderStatusDto();
        updateOrderStatus.OrderId = request.OrderId;
        updateOrderStatus.InvoiceNo = orderById.InvoiceNo;

        if (currentOrderStatus?.OrderStatusValueId != null) updateOrderStatus.NewOrderStatus = (int)currentOrderStatus.OrderStatusValueId;
        if (currentOrderStatus?.OrderStatusValue != null) updateOrderStatus.CurrentOrderStatus = currentOrderStatus?.OrderStatusValue?.StatusValue;
        if (currentOrderStatus?.LastModifiedDate != null) updateOrderStatus.CurrentOrderStatusTime = currentOrderStatus?.LastModifiedDate;

        var orderStatus = await _db.OrderStatus.AsNoTracking().Include(o => o.OrderStatusValue).Where(o => o.OrderId == request.OrderId).OrderBy(o => o.Id).ToListAsync(cancellationToken);
        var customer = await _db.Customers.Include(c=>c.User).Where(o => o.Id == orderById.CustomerId).FirstOrDefaultAsync(cancellationToken);
        var customerInfo = _mapper.Map<CustomerDto>(customer);
        var orderDetails = new OrderInfoDetailsDto
        {
            Order = orderById,
            CurrentOrderStatus = currentOrderStatus,
            OrderStatus = orderStatus,
            OrderItemDetails = orderItemDetails,
            UpdateOrderStatus = updateOrderStatus,
            CustomerInfo = customerInfo
        };

        return orderDetails;
    }
}
