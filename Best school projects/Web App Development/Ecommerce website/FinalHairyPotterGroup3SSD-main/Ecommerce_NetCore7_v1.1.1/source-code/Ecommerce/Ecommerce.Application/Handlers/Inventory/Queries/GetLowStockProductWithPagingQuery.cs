using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto.Stock;
using Ecommerce.Application.Helpers;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Inventory.Queries;

public class GetLowStockProductWithPagingQuery : IRequest<PaginatedList<LowStockProductDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "VariantId";
    public string sortOrder { get; set; } = "Desc";
}
public class GetLowStockProductWithPagingQueryHandler : IRequestHandler<GetLowStockProductWithPagingQuery, PaginatedList<LowStockProductDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetLowStockProductWithPagingQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<PaginatedList<LowStockProductDto>> Handle(GetLowStockProductWithPagingQuery request, CancellationToken cancellationToken)
    {
        StockConfiguration conStock = JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection(AppConfigurationType.StockConfiguration));

        //var variants = _db.Variants.Where(o => o.Qty <= conStock.OutOfStockThreshold).AsQueryable();
        var stocks = _db.Products.Include(c => c.Category).Join(
                    _db.Variants,
                    product => product.Id,
                    variant => variant.ProductId,
                (product, variant) => new
                {
                    product = product,
                    variant = variant
                })
                .Where(o => o.variant.Qty <= conStock.OutOfStockThreshold)
                .Select(entity => new LowStockProductDto()
                {
                    ProductId = entity.product.Id,
                    VariantId = entity.variant.Id,
                    ProductTitle = entity.product.Name,
                    VariantTitle = entity.variant.Title,
                    Category = entity.product.Category.Name,
                    Qty = entity.variant.Qty,
                    LowStockVariantCount = entity.variant.Qty,
                })
                .AsQueryable();

        var getStocks =
            stocks
                .Where(a => a.VariantTitle.ToLower().Contains(request.searchValue) || a.Category.ToLower().Contains(request.searchValue) || a.Qty.ToString().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .AsQueryable();



        var data = await PaginatedList<LowStockProductDto>.CreateAsync(getStocks, request.page ?? 1, request.length);

        return data;
    }
}
