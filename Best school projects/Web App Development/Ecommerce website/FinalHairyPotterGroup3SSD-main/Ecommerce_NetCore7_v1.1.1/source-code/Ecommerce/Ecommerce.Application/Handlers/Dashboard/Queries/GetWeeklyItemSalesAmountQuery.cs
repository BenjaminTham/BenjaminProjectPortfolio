using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Dashboard.Queries;

public class GetWeeklyItemSalesAmountQuery : IRequest<List<WeeklySalesAmount>>
{
}
public class GetWeeklyItemSalesAmountQueryHandler : IRequestHandler<GetWeeklyItemSalesAmountQuery, List<WeeklySalesAmount>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetWeeklyItemSalesAmountQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<WeeklySalesAmount>> Handle(GetWeeklyItemSalesAmountQuery request, CancellationToken cancellationToken)
    {
        List<Order> thisWeeksSales = await _db.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
        .Where(o => o.CreatedDate >= DateTime.Today.AddDays(-6) && o.CreatedDate < DateTime.Today.AddDays(1)).ToListAsync();

        List<WeeklySalesAmount> weeklySalesAmount = new()
        {
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(0).Date, NameOfDay = $"(Today){DateTime.Today.AddDays(0).ToString("ddd")}", OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(0) && o.CreatedDate < DateTime.Today.AddDays(1)).Select(o => o.OrderAmount).Sum(), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(0) && o.CreatedDate < DateTime.Today.AddDays(1)).Select(o => o.DeliveryCharge).Sum() },
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(-1).Date, NameOfDay = DateTime.Today.AddDays(-1).ToString("ddd"), OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-1) && o.CreatedDate < DateTime.Today.AddDays(0)).Select(o => o.OrderAmount).Sum(), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-1) && o.CreatedDate < DateTime.Today.AddDays(0)).Select(o => o.DeliveryCharge).Sum() },
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(-2).Date, NameOfDay = DateTime.Today.AddDays(-2).ToString("ddd"), OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-2) && o.CreatedDate < DateTime.Today.AddDays(-1)).Sum(o => o.OrderAmount + (decimal)o.DeliveryCharge), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-2) && o.CreatedDate < DateTime.Today.AddDays(-1)).Select(o => o.DeliveryCharge).Sum() },
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(-3).Date, NameOfDay = DateTime.Today.AddDays(-3).ToString("ddd"), OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-3) && o.CreatedDate < DateTime.Today.AddDays(-2)).Sum(o => o.OrderAmount + (decimal)o.DeliveryCharge), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-3) && o.CreatedDate < DateTime.Today.AddDays(-2)).Select(o => o.DeliveryCharge).Sum() },
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(-4).Date, NameOfDay = DateTime.Today.AddDays(-4).ToString("ddd"), OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-4) && o.CreatedDate < DateTime.Today.AddDays(-3)).Sum(o => o.OrderAmount + (decimal)o.DeliveryCharge), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-4) && o.CreatedDate < DateTime.Today.AddDays(-3)).Select(o => o.DeliveryCharge).Sum() },
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(-5).Date, NameOfDay = DateTime.Today.AddDays(-5).ToString("ddd"), OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-5) && o.CreatedDate < DateTime.Today.AddDays(-4)).Sum(o => o.OrderAmount + (decimal)o.DeliveryCharge), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-5) && o.CreatedDate < DateTime.Today.AddDays(-4)).Select(o => o.DeliveryCharge).Sum() },
            new WeeklySalesAmount { OrderDate = DateTime.Today.AddDays(-6).Date, NameOfDay = DateTime.Today.AddDays(-6).ToString("ddd"), OrderAmount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-6) && o.CreatedDate < DateTime.Today.AddDays(-5)).Sum(o => o.OrderAmount + (decimal)o.DeliveryCharge), DeliveryCharge = (decimal)thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-6) && o.CreatedDate < DateTime.Today.AddDays(-5)).Select(o => o.DeliveryCharge).Sum() },
        };

        return weeklySalesAmount.OrderBy(o => o.OrderDate).ToList();
    }
}
