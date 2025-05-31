using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Dashboard.Queries;

public class GetWeeklyItemSalesCountQuery : IRequest<List<WeeklyItemSalesCount>>
{
}
public class GetWeeklyItemSalesCountQueryHandler : IRequestHandler<GetWeeklyItemSalesCountQuery, List<WeeklyItemSalesCount>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetWeeklyItemSalesCountQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<WeeklyItemSalesCount>> Handle(GetWeeklyItemSalesCountQuery request, CancellationToken cancellationToken)
    {
        List<Order> thisWeeksSales = await _db.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.OrderStatus)
            .ThenInclude(o => o.OrderStatusValue)
        .Where(o => o.CreatedDate >= DateTime.Today.AddDays(-6) && o.CreatedDate < DateTime.Today.AddDays(1)).ToListAsync();

        List<WeeklyItemSalesCount> weeklyItemSalesCount = new()
        {
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(0).Date, NameOfDay = $"(Today){DateTime.Today.AddDays(0).ToString("ddd")}", ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(0) && o.CreatedDate < DateTime.Today.AddDays(1)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(0) && o.CreatedDate < DateTime.Today.AddDays(1)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(-1).Date, NameOfDay = DateTime.Today.AddDays(-1).ToString("ddd"), ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-1) && o.CreatedDate < DateTime.Today.AddDays(0)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-1) && o.CreatedDate < DateTime.Today.AddDays(0)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(-2).Date, NameOfDay = DateTime.Today.AddDays(-2).ToString("ddd"), ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-2) && o.CreatedDate < DateTime.Today.AddDays(-1)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-2) && o.CreatedDate < DateTime.Today.AddDays(-1)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(-3).Date, NameOfDay = DateTime.Today.AddDays(-3).ToString("ddd"), ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-3) && o.CreatedDate < DateTime.Today.AddDays(-2)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-3) && o.CreatedDate < DateTime.Today.AddDays(-2)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(-4).Date, NameOfDay = DateTime.Today.AddDays(-4).ToString("ddd"), ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-4) && o.CreatedDate < DateTime.Today.AddDays(-3)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-4) && o.CreatedDate < DateTime.Today.AddDays(-3)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(-5).Date, NameOfDay = DateTime.Today.AddDays(-5).ToString("ddd"), ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-5) && o.CreatedDate < DateTime.Today.AddDays(-4)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-5) && o.CreatedDate < DateTime.Today.AddDays(-4)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
            new WeeklyItemSalesCount { OrderDate = DateTime.Today.AddDays(-6).Date, NameOfDay = DateTime.Today.AddDays(-6).ToString("ddd"), ItemCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-6) && o.CreatedDate < DateTime.Today.AddDays(-5)).Select(o => o.OrderDetails.Select(o => o.Qty).Sum()).Sum(), ProductCount = thisWeeksSales.Where(o => o.CreatedDate >= DateTime.Today.AddDays(-6) && o.CreatedDate < DateTime.Today.AddDays(-5)).SelectMany(o => o.OrderDetails.Select(o => o.ProductVariantId)).Distinct().Count() },
        };

        return weeklyItemSalesCount.OrderBy(o => o.OrderDate).ToList();
    }
}
