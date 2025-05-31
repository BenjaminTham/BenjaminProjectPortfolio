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


namespace Ecommerce.Application.Handlers.RenderItems.Queries;

public class GetFeatureProductQuery : IRequest<IEnumerable<FeatureProductShowcaseDto>>
{
}
public class GetFeatureProductQueryHandler : IRequestHandler<GetFeatureProductQuery, IEnumerable<FeatureProductShowcaseDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetFeatureProductQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<IEnumerable<FeatureProductShowcaseDto>> Handle(GetFeatureProductQuery request, CancellationToken cancellationToken)
    {
        var conStock = _keyAccessor?["StockConfiguration"] is not null
            ? JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection("StockConfiguration"))!
            : new StockConfiguration();

        var conFeatProduct = _keyAccessor?["FeatureProductConfiguration"] is not null
            ? JsonSerializer.Deserialize<List<FeatureProductShowcaseDto>>(_keyAccessor.GetSection("FeatureProductConfiguration"))!
            : new List<FeatureProductShowcaseDto>();

        
        if (conFeatProduct == null) return new List<FeatureProductShowcaseDto>();
        var featProductIds = conFeatProduct.Select(c => c.ProductId).ToList();

        var outOfStockThreshold = conStock.IsOutOfStockItemHidden ? conStock.OutOfStockThreshold : null;

        var d = await GetShopList(outOfStockThreshold, featProductIds);
        var featProduct = d.GroupBy(w => w.Id, (key, value) => new FeatureProductShowcaseDto
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

        return featProduct;
    }
    public async Task<List<ProductShopItemsDto>> GetShopList(int? outOfStockThreshold, List<long> featProductIds)
    {
        var query = (from p in _db.Products
                     join vr in _db.Variants on p.Id equals vr.ProductId into jvr
                     from variant in jvr.DefaultIfEmpty()
                     join c in _db.Categories on p.CategoryId equals c.Id into jc
                     from category in jc.DefaultIfEmpty()
                     join col in _db.Colors on variant.ColorId equals col.Id into jcol
                     from color in jcol.DefaultIfEmpty()
                     join siz in _db.Sizes on variant.SizeId equals siz.Id into jsiz
                     from size in jsiz.DefaultIfEmpty()
                     join pImg in _db.ProductImages on p.Id equals pImg.ProductId into jpImg
                     from productImage in jpImg.DefaultIfEmpty()
                     join vaImg in _db.VariantImages on variant.Id equals vaImg.VariantId into jvaImg
                     from variantImage in jvaImg.DefaultIfEmpty()
                     where (outOfStockThreshold == null || variant.Qty > outOfStockThreshold)
                     && featProductIds.Contains(p.Id)
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
                         ProductImage = _db.Galleries.Where(g => g.Id == productImage.ImageId).FirstOrDefault().Name,
                     }).AsQueryable();

        query = query.OrderByDescending(c => c.Id);

        var filteredItems = query
            .OrderByDescending(c => c.Id)
            .Select(c => c.Id).Distinct()
            .Take(2).ToList();

        var result = query.Where(t => filteredItems.Contains(t.Id)).ToList();
        return (result);
    }

}
