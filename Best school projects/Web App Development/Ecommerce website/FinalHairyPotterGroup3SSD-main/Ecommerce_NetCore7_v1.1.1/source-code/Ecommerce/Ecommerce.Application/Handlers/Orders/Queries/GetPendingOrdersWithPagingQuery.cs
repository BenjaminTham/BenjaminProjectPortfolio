using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetPendingOrdersWithPagingQuery : IRequest<PaginatedList<OrderDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetPendingOrdersWithPagingQueryHandler : IRequestHandler<GetPendingOrdersWithPagingQuery, PaginatedList<OrderDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetPendingOrdersWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OrderDto>> Handle(GetPendingOrdersWithPagingQuery request, CancellationToken cancellationToken)
    {
        var pendingOrderStatus = await _db.OrderStatusValues.Where(o => o.StatusValue == DefaultOrderStatusValue.Pending().StatusValue).FirstOrDefaultAsync();
        var orders = _db.Orders.AsNoTracking().Include(o => o.OrderDetails)
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
            .Where(o => o.OrderStatus.OrderByDescending(o => o.Id).Select(o => o.OrderStatusValueId).AsQueryable().FirstOrDefault() == pendingOrderStatus.Id)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                InvoiceNo = o.InvoiceNo,
                CustomerId = o.CustomerId,
                CustomerName = o.CustomerName,
                ShippingAddress = o.ShippingAddress,
                OrderAmount = o.OrderAmount,
                PaymentStatus = o.PaymentStatus,
                DeliveryCharge = o.DeliveryCharge,
                PaymentMethod = o.PaymentMethod,
                CreatedDate = o.CreatedDate,
                CurrentOrderStatus = o.OrderStatus.OrderByDescending(o => o.Id).Select(o => o.OrderStatusValue).AsQueryable().FirstOrDefault().StatusValue
            })
            .AsQueryable();

        var getOrders =
                orders
                .Where(a => a.CustomerName.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}");

        var data = await PaginatedList<OrderDto>.CreateAsync(getOrders, request.page ?? 1, request.length);
        return data;
    }
}
