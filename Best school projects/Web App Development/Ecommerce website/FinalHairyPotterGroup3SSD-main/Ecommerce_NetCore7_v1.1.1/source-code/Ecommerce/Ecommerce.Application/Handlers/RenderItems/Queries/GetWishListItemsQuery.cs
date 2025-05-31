using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Models;
using MediatR;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.RenderItems.Queries;

public class GetWishListItemsQuery : IRequest<IEnumerable<WishListDto>>
{
}
public class GetWishListItemsQueryHandler : IRequestHandler<GetWishListItemsQuery, IEnumerable<WishListDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    private readonly ICookieService _cookie;
    public GetWishListItemsQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor, ICookieService cookie)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
        _cookie = cookie;
    }

    public async Task<IEnumerable<WishListDto>> Handle(GetWishListItemsQuery request, CancellationToken cancellationToken)
    {
        long[] wishListPIds;
        var myString = _cookie.Get("wish-list");
        if (_cookie.Contains("wish-list"))
            wishListPIds = myString.Split(',').Select(long.Parse).ToArray();
        else return new List<WishListDto>();

        var conStock = _keyAccessor?["StockConfiguration"] is not null
            ? JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection("StockConfiguration"))!
            : new StockConfiguration();


        var outOfStockThreshold = conStock.IsOutOfStockItemHidden ? conStock.OutOfStockThreshold : null;

        var getShop = await GetShopList(outOfStockThreshold, wishListPIds);
        var newProduct = getShop.GroupBy(w => w.Id, (key, value) => new WishListDto
        {
            ProductId = key,
            Name = value.First().ProductName,
            Slug = value.First().Slug,
            ProductImage = value.First().ProductImage,
            CategoryId = value.First().CategoryId,
            CategoryName = value.First().CategoryName,
            Price = MinMaxVal.getMinMaxVal(value.Where(c => c.Id == key).Select(o => o.Price).ToArray())
        }).ToList();

        return newProduct;
    }
    public async Task<List<ProductShopItemsDto>> GetShopList(int? outOfStockThreshold, long[] pIds)
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
                     where pIds.Contains(p.Id)
                           && (outOfStockThreshold == null || variant.Qty > outOfStockThreshold)
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

        var filteredItems = await query
            .OrderByDescending(c => c.Id)
            .Select(c => c.Id).Distinct()
            .Take(5).ToListAsync();

        var result = await query.Where(t => filteredItems.Contains(t.Id)).ToListAsync();
        return (result);
    }
}
