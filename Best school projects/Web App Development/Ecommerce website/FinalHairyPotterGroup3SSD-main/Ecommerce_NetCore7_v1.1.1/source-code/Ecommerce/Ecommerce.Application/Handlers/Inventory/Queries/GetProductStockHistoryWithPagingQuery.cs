using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Dto.Stock;
using Ecommerce.Application.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Inventory.Queries;

public class GetProductStockHistoryWithPagingQuery : IRequest<PaginatedList<ProductStockHistoryDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetProductStockHistoryWithPagingQueryHandler : IRequestHandler<GetProductStockHistoryWithPagingQuery, PaginatedList<ProductStockHistoryDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductStockHistoryWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductStockHistoryDto>> Handle(GetProductStockHistoryWithPagingQuery request, CancellationToken cancellationToken)
    {
        var history = _db.Stocks.Include(o => o.Variant).Select(c=> new ProductStockHistoryDto
        {
            Id = c.Id,
            VariantId = c.VariantId,
            VariantTitle = c.Variant.Title,
            Qty = c.Qty,
            Description = c.Description,
            StockInputType = c.StockInputType,
            StockInputTypeName = c.StockInputType.ToString(),
            LastModifiedDate = c.LastModifiedDate
        }).OrderByDescending(o => o.LastModifiedDate).AsQueryable();

        try
        {
            var getHistorsy =
                history
                    .Where(a => a.VariantTitle.ToLower().Contains(request.searchValue))
                    .OrderBy($"{request.sortColumn} {request.sortOrder}").AsQueryable();
            var datda = await PaginatedList<ProductStockHistoryDto>.CreateAsync(getHistorsy, request.page ?? 1, request.length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var getHistory =
            history
                .Where(a => a.VariantTitle.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}").AsQueryable();

        var data = await PaginatedList<ProductStockHistoryDto>.CreateAsync(getHistory, request.page ?? 1, request.length);
        return data;
    }
}
