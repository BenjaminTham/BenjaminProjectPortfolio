using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Models;
using MediatR;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.RenderItems.Queries;

public class GetNewProductQuery : IRequest<IEnumerable<NewProductShocaseDto>>
{
    public int itemQty { get; set; } = 5;
}
public class GetNewProductQueryHandler : IRequestHandler<GetNewProductQuery, IEnumerable<NewProductShocaseDto>>
{
    private readonly IDataContext _db;
    private readonly IKeyAccessor _keyAccessor;
    public GetNewProductQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _keyAccessor = keyAccessor;
    }

    public async Task<IEnumerable<NewProductShocaseDto>> Handle(GetNewProductQuery request, CancellationToken cancellationToken)
    {
        var conStock = _keyAccessor?["StockConfiguration"] is not null
            ? JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection("StockConfiguration"))!
            : new StockConfiguration();

        var outOfStockThreshold = conStock.IsOutOfStockItemHidden ? conStock.OutOfStockThreshold : null;

        var getShop = await GetShopList(outOfStockThreshold, request.itemQty);
        var newProduct = getShop.GroupBy(w => w.Id, (key, value) => new NewProductShocaseDto
        {
            ProductId = key,
            Name = value.First().ProductName,
            Slug = value.First().Slug,
            ProductImage = value.First().ProductImage,
            CategoryId = value.First().CategoryId,
            CategoryName = value.First().CategoryName,
            ShortDescription = value.First().ShortDescription,
            Price = MinMaxVal.getMinMaxVal(value.Where(c => c.Id == key).Select(o => o.Price).ToArray())
        }).ToList();

        return newProduct;
    }
    public async Task<List<ProductShopItemsDto>> GetShopList(int? outOfStockThreshold, int take)
    {
        var productIds = await _db.Products
            .Join(_db.Variants, p => p.Id, v => v.ProductId, (p, v) => new { p, v })
            .Where(x => outOfStockThreshold == null || x.v.Qty > outOfStockThreshold)
            .Select(x => x.p.Id)
            .Distinct().OrderByDescending(x => x).Take(take).ToListAsync();

        var dd = productIds;


        var query = (from p in _db.Products
                     join vr in _db.Variants on p.Id equals vr.ProductId into jvr
                     from variant in jvr.DefaultIfEmpty()
                     join c in _db.Categories on p.CategoryId equals c.Id into jc
                     from category in jc.DefaultIfEmpty()
                     join col in _db.Colors on variant.ColorId equals col.Id into jcol
                     from color in jcol.DefaultIfEmpty()
                     join siz in _db.Sizes on variant.SizeId equals siz.Id into jsiz
                     from size in jsiz.DefaultIfEmpty()
                     join pImg in _db.ProductImages on p.Id equals pImg.ProductId into pImgGroup
                     from pImg in pImgGroup.DefaultIfEmpty()
                     join g in _db.Galleries on pImg.ImageId equals g.Id into gGroup
                     from g in gGroup.DefaultIfEmpty()
                     where productIds.Contains(p.Id)
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


        var result = await query.OrderByDescending(c => c.Id).ToListAsync();
        return (result);
    }
}
