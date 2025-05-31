using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Dto.Shop;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Identity.Entities;
using Ecommerce.Domain.Models;
using Ecommerce.Domain.Utilities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Shop.Queries;

public class GetProductDetailsBySlug : IRequest<ShopDetailsDto>
{
    public string Slug { get; set; }
}
public class GetProductDetailsBySlugHandler : IRequestHandler<GetProductDetailsBySlug, ShopDetailsDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    public GetProductDetailsBySlugHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
        _userManager = userManager;
    }

    public async Task<ShopDetailsDto> Handle(GetProductDetailsBySlug request, CancellationToken cancellationToken)
    {
        StockConfiguration conStock = JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection("StockConfiguration"))!;
        var outOfStockThreshold = conStock.IsOutOfStockItemHidden ? conStock.OutOfStockThreshold : null;

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
                           && p.Slug == request.Slug
                     select new ProductShopItemsDto
                     {
                         Id = p.Id,
                         ProductName = p.Name,
                         Slug = p.Slug,
                         KeySpecs = p.KeySpecs,
                         ShortDescription = p.ShortDescription,
                         Description = p.Description,
                         VariableTheme = p.VariableTheme,
                         CategoryId = p.CategoryId,
                         CategoryName = category.Name,
                         CategorySlug = category.Slug,
                         VariantId = variant.Id,
                         VariantTitle = variant.Title,
                         Sku = variant.Sku,
                         ColorId = variant.ColorId,
                         ColorName = color.Name,
                         ColorCode = color.HexCode,
                         SizeId = variant.SizeId,
                         SizeName = size.Name,
                         Price = variant.Price,
                         Qty = variant.Qty,
                         ProductImage = _db.Galleries.Where(g => g.Id == productImage.ImageId).FirstOrDefault().Name,
                         VariantImage = _db.Galleries.Where(g => g.Id == variantImage.ImageId).FirstOrDefault().Name
                     });


        var product = await query.ToListAsync(cancellationToken);

        var availableColor = product.Where(c => c.ColorId != null).Select(c => new ColorDto { Id = ProjectUtilities.TryParseNullableInt(c.ColorId), Name = c.ColorName, HexCode = c.ColorCode }).DistinctBy(c => c.Id).ToList();
        var availableSize = product.Where(c => c.SizeId != null).Select(c => new SizeDto { Id = ProjectUtilities.TryParseNullableInt(c.SizeId), Name = c.SizeName }).DistinctBy(c => c.Id).ToList();
        var availableImages = product.Where(c => c.VariantImage != null).Select(c => new GalleryPreviewDto { Url = c.VariantImage }).Distinct().ToList();
        var colorSizeCombination = product.Where(c => c.ColorId != null || c.SizeId != null).Select(c =>
        new ColorSizeDto
        {
            VariantId = c.VariantId,
            ColorId = ProjectUtilities.TryParseNullableInt(c.ColorId),
            SizeId = ProjectUtilities.TryParseNullableInt(c.SizeId),
        }).ToList();

        var result = new ShopDetailsDto
        {
            ProductDetails = product.FirstOrDefault() ?? new ProductShopItemsDto(),
            AvailableColors = availableColor,
            AvailableSizes = availableSize,
            AvailableImages = availableImages,
            ColorSizeCombination = colorSizeCombination
        };

        return result;
    }
}
