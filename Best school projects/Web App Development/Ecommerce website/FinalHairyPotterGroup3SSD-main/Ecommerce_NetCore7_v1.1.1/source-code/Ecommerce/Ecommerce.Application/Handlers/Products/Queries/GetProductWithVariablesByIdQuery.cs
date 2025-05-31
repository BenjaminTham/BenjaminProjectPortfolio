using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace Ecommerce.Application.Handlers.Products.Queries;

public class GetProductWithVariablesByIdQuery : IRequest<ProductForEditDto>
{
    public int Id { get; set; }
}
public class GetProductWithVariablesByIdQueryHandler : IRequestHandler<GetProductWithVariablesByIdQuery, ProductForEditDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductWithVariablesByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductForEditDto> Handle(GetProductWithVariablesByIdQuery request, CancellationToken cancellationToken)
    {
        var productVariantVm = await (from pv in _db.Variants
            join pvImg in _db.VariantImages on pv.Id equals pvImg.VariantId into pvImgGroup
            from pvImg in pvImgGroup.DefaultIfEmpty()
            join g in _db.Galleries on pvImg.ImageId equals g.Id into gGroup
            from g in gGroup.DefaultIfEmpty()


            where pv.ProductId == request.Id
            //let vi = _db.VariantImages.FirstOrDefault(img => img.VariantId == pv.Id)
            //let i = _db.Galleries.FirstOrDefault(img => img.Id == vi.ImageId)
            select new ProductVariantForEditDto
            {
                Id = pv.Id,
                Title = pv.Title,
                ProductId = pv.ProductId,
                SizeId = pv.SizeId,
                ColorId = pv.ColorId,
                Sku = pv.Sku,
                Price = pv.Price,
                Qty = pv.Qty,
                VariantImageId = pvImg != null ? pvImg.ImageId : null,
                VariantImagePreview = g != null ? g.Name : null
            }).ToListAsync(cancellationToken);

        var productVm = await (from p in _db.Products
            join pImg in _db.ProductImages on p.Id equals pImg.ProductId into pImgGroup
            from pImg in pImgGroup.DefaultIfEmpty()
            join g in _db.Galleries on pImg.ImageId equals g.Id into gGroup
            from g in gGroup.DefaultIfEmpty()
            where p.Id == request.Id
            select new ProductForEditDto
            {
                ProductId = p.Id,
                CategoryId = p.CategoryId,
                Name = p.Name,
                Slug = p.Slug,
                KeySpecs = p.KeySpecs,
                ShortDescription = p.ShortDescription,
                Description = p.Description,
                VariableTheme = p.VariableTheme,
                ProductImage = pImg != null ? pImg.ImageId : null,
                ProductImagePreview = g != null ? g.Name : null,
                ProductVariant = productVariantVm.Any() ? productVariantVm : null
            }).FirstOrDefaultAsync(cancellationToken);


        return productVm;
    }
}
