using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Shop.Queries;

public class GetShopWithPagingQuery : IRequest<ShopDto>
{
    public string? color { get; set; }
    public string? size { get; set; }
    public int? page { get; set; }
    public int pageSize { get; set; } = 12;
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetShopWithPagingQueryHandler : IRequestHandler<GetShopWithPagingQuery, ShopDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetShopWithPagingQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }


    public async Task<ShopDto> Handle(GetShopWithPagingQuery request, CancellationToken cancellationToken)
    {
        var conStock = _keyAccessor?["StockConfiguration"] is not null
            ? JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection("StockConfiguration"))!
            : new StockConfiguration();

        var outOfStockThreshold = conStock.IsOutOfStockItemHidden ? conStock.OutOfStockThreshold : null;
        var skip = (request.page - 1) * request.pageSize;
        var take = request.pageSize;

        var (d, count) = await GetShopList(request.color, request.size, outOfStockThreshold, skip, take, request.sortColumn, request.sortOrder);
        var shopDto = d.GroupBy(w => w.Id, (key, value) => new ShopShowcaseDto
        {
            ProductId = key,
            Name = value.First().ProductName,
            Slug = value.First().Slug,
            ProductImage = value.First().ProductImage,
            CategoryId = value.First().CategoryId,
            CategoryName = value.First().CategoryName,
            ShortDescription = value.First().ShortDescription,
            AvailableColorVariant = value.Where(c => c.ColorId is not null)
                .Select(c => new ColorDto { Id = (int)c.ColorId, Name = c.ColorName, HexCode = c.ColorCode })
                .DistinctBy(c => c.Id).ToList(),
            AvailableSizesVariant = value.Where(c => c.SizeId is not null)
                .Select(c => new SizeDto { Id = (int)c.SizeId, Name = c.SizeName })
                .DistinctBy(c => c.Id).ToList(),
            Price = MinMaxVal.getMinMaxVal(value.Where(c => c.Id == key).Select(o => o.Price).ToArray())
        }).ToList();

        var data = PaginatedList<ShopShowcaseDto>.CreateStatic(shopDto, count, request.page ?? 1, request.pageSize);
        return new ShopDto { PaginatedProductList = data };
    }

    public async Task<(List<ProductShopItemsDto>, int)> GetShopList(string? colorId, string? sizeId, int? outOfStockThreshold, int? skip, int take, string sortColumn, string sortOrder)
    {
        var colorIds = colorId?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
        var sizeIds = sizeId?.Split(',').Select(int.Parse).ToList() ?? new List<int>();

        var query = (from p in _db.Products
                     join variant in _db.Variants on p.Id equals variant.ProductId into jvr
                     from variant in jvr.DefaultIfEmpty()
                     join category in _db.Categories on p.CategoryId equals category.Id into jc
                     from category in jc.DefaultIfEmpty()
                     join color in _db.Colors on variant.ColorId equals color.Id into jcol
                     from color in jcol.DefaultIfEmpty()
                     join size in _db.Sizes on variant.SizeId equals size.Id into jsiz
                     from size in jsiz.DefaultIfEmpty()
                     join pImg in _db.ProductImages on p.Id equals pImg.ProductId into pImgGroup
                     from pImg in pImgGroup.DefaultIfEmpty()
                     join g in _db.Galleries on pImg.ImageId equals g.Id into gGroup
                     from g in gGroup.DefaultIfEmpty()

                     where (outOfStockThreshold == null || variant.Qty > outOfStockThreshold)
                           && (colorId == null || colorIds.Contains((int)variant.ColorId))
                           && (sizeId == null || sizeIds.Contains((int)variant.SizeId))
                     select new ProductShopItemsDto
                     {
                         Id = p.Id,
                         ProductName = p.Name,
                         Slug = p.Slug,
                         ShortDescription = p.ShortDescription,
                         CategoryId = p.CategoryId,
                         CategoryName = category.Name,
                         CategorySlug = category.Slug,
                         VariantId = variant.Id,
                         ColorId = variant.ColorId,
                         ColorName = color.Name,
                         ColorCode = color.HexCode,
                         SizeId = variant.SizeId,
                         SizeName = size.Name,
                         Price = variant.Price,
                         Qty = variant.Qty,
                         ProductImage = g != null ? g.Name : null
                     }).AsQueryable();

        query = query.OrderBy($"{sortColumn} {sortOrder}");

        var filteredItems = query
            .OrderBy($"{sortColumn} {sortOrder}")
            .Select(c => c.Id).Distinct()
            .Skip(skip ?? 0).Take(take).ToList();

        var result = query.Where(t => filteredItems.Contains(t.Id)).ToList();
        var count = await query.Select(t => t.Id).Distinct().CountAsync();
        result.ForEach(c => c.Count = count);

        return (result, count);
    }
}


