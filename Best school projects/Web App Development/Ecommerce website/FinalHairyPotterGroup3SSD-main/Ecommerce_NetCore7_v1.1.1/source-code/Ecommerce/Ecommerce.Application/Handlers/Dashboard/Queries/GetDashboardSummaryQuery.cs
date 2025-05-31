using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Dashboard.Queries;

public class GetDashboardSummaryQuery : IRequest<DashboardDto>
{
}
public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetDashboardSummaryQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<DashboardDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        StockConfiguration conStock = JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection(AppConfigurationType.StockConfiguration))!;
        var pendingOrderStatus = await _db.OrderStatusValues.Where(o => o.StatusValue == DefaultOrderStatusValue.Pending().StatusValue).FirstOrDefaultAsync(cancellationToken);
        var pendingCount = await _db.Orders
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
            .Where(o => o.OrderStatus.OrderByDescending(o => o.Id).Select(o => o.OrderStatusValueId).FirstOrDefault() == pendingOrderStatus.Id).CountAsync(cancellationToken);


        List<Order> salesToday = await _db.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
        .Where(o => o.CreatedDate >= DateTime.Today.AddDays(0) && o.CreatedDate < DateTime.Today.AddDays(1)).ToListAsync(cancellationToken);

        

        var lowStockCount = await _db.Variants.Where(o => o.Qty <= conStock.OutOfStockThreshold).CountAsync(cancellationToken);

        DashboardDto dashboard = new DashboardDto();
        dashboard.PendingOrderCount = pendingCount;
        dashboard.LowStockItemCount = lowStockCount;

        TodaySalesSummary salesSummary = new()
        {
            TotalProduct = salesToday.SelectMany(o => o.OrderDetails.Select(p => p.ProductVariantId)).Distinct().Count(),
            TotalItem = salesToday.Select(o => o.OrderDetails.Select(p => p.Qty).Sum()).Sum(),
            TotalSales = salesToday.Select(o => o.Id).Count(),
            TotalSalesAmount = salesToday.Select(o => o.OrderAmount).Sum(),
            TotalDeliveryCharge = (decimal)salesToday.Select(o => o.DeliveryCharge).Sum(),
        };

        CustomerInfo customerInfo = new()
        {
            TotalCustomer = await _db.Customers.CountAsync(cancellationToken),
            TotalCustomerToday = await _db.Customers.CountAsync(o => o.CreatedDate >= DateTime.Today.AddDays(0) && o.CreatedDate < DateTime.Today.AddDays(1), cancellationToken)
        };

        dashboard.TodaySalesSummary = salesSummary;
        dashboard.CustomerInfo = customerInfo;

        return dashboard;
    }
}
