using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Shop.Queries;

public class GetProductDetailsFilterResultQuery : IRequest<ProductDetailsFilterResultDto>
{
    public int Id { get; set; }
    public int Color { get; set; }
    public int Size { get; set; }
}
public class GetProductDetailsFilterResultQueryHandler : IRequestHandler<GetProductDetailsFilterResultQuery, ProductDetailsFilterResultDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductDetailsFilterResultQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDetailsFilterResultDto> Handle(GetProductDetailsFilterResultQuery request, CancellationToken cancellationToken)
    {
        //int? colorId = request?.Color!=null? Int32.Parse(request.Color) : null;
        //int? sizeId = request?.Size!=null? Int32.Parse(request.Size) : null;

        var query = (from variant in _db.Variants
                     join col in _db.Colors on variant.ColorId equals col.Id into jcol
                     from color in jcol.DefaultIfEmpty()
                     join siz in _db.Sizes on variant.SizeId equals siz.Id into jsiz
                     from size in jsiz.DefaultIfEmpty()
                     join vaImg in _db.VariantImages on variant.Id equals vaImg.VariantId into jvaImg
                     from variantImage in jvaImg.DefaultIfEmpty()
                     where variant.ProductId == request.Id
                           //&& (request.Size > 0 ? (variant.SizeId == request.Size) : true)
                           //&& (request.Color > 0 ? (variant.ColorId == request.Color) : true)
                           && (request.Size <= 0 || (variant.SizeId == request.Size))
                           && (request.Color <= 0 || (variant.ColorId == request.Color))
                     select new ProductDetailsFilterResultDto
                     {
                         VariantId = variant.Id,
                         Sku = variant.Sku,
                         Qty = variant.Qty,
                         Price = variant.Price,
                         ColorId = ProjectUtilities.TryParseNullableInt(variant.ColorId),
                         SizeId = ProjectUtilities.TryParseNullableInt(variant.SizeId),
                         VariantImage = _db.Galleries.Where(g => g.Id == variantImage.ImageId).FirstOrDefault().Name
                     });

        var filterResult = await query.FirstOrDefaultAsync(cancellationToken);
        return filterResult;
    }
}
