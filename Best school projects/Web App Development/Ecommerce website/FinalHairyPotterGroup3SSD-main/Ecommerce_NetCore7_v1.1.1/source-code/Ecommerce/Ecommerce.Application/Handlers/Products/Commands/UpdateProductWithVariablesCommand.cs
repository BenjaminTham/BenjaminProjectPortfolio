using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Products.Commands;

public class UpdateProductWithVariablesCommand : IRequest<Response<string>>
{
    public ProductForEditDto ProductForEditDto { get; set; }
}
public class UpdateProductWithVariablesCommandHandler : IRequestHandler<UpdateProductWithVariablesCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateProductWithVariablesCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateProductWithVariablesCommand request, CancellationToken cancellationToken)
    {

        #region Experimental Code Refactor

        var pVariable = await _db.Variants.Where(o => o.ProductId == request.ProductForEditDto.ProductId).ToListAsync(cancellationToken);
        var addableVariable = request.ProductForEditDto.ProductVariant.Where(c => c.Id == 0).ToList();
        var updateableVariable = request.ProductForEditDto.ProductVariant.Where(c => c.Id > 0).ToList();
        var updateableVariableId = updateableVariable.Select(c => c.Id).ToList();
        var removableVariable = pVariable.Where(c => !updateableVariableId.Contains(c.Id)).ToList();

        //var product = await _db.Products.FindAsync(request.ProductForEditDto.ProductId);
        //if (product == null) return Response<string>.Fail("Sorry! No Product Found To Update");

        //product.Name = request.ProductForEditDto.Name;
        //product.Slug = request.ProductForEditDto.Slug;
        //product.CategoryId = request.ProductForEditDto.CategoryId;
        //product.ShortDescription = request.ProductForEditDto.ShortDescription;
        //product.Description = request.ProductForEditDto.Description;
        //product.VariableTheme = request.ProductForEditDto.VariableTheme;  
        //_db.Products.Update(product);

        //_db.Variants.RemoveRange(removableVariable); 

        #endregion




        try
        {
            var varRemove = await _db.Variants.Where(o => o.ProductId == request.ProductForEditDto.ProductId).ToListAsync(cancellationToken);
            _db.Variants.RemoveRange(varRemove);

            var product = await _db.Products.FindAsync(request.ProductForEditDto.ProductId);
            if (product == null) return Response<string>.Fail("Sorry! No Product Found To Update");

            product.Name = request.ProductForEditDto.Name;
            product.Slug = request.ProductForEditDto.Slug;
            product.KeySpecs = request.ProductForEditDto.KeySpecs;
            product.CategoryId = request.ProductForEditDto.CategoryId;
            product.ShortDescription = request.ProductForEditDto.ShortDescription;
            product.Description = request.ProductForEditDto.Description;
            product.VariableTheme = request.ProductForEditDto.VariableTheme;

            _db.Products.Update(product);

            if (request?.ProductForEditDto.ProductImage != null)
            {
                ProductImage productImage = new ProductImage();
                var proImaRemove = await _db.ProductImages.Where(o => o.ProductId == request.ProductForEditDto.ProductId).ToListAsync(cancellationToken);
                _db.ProductImages.RemoveRange(proImaRemove);

                productImage.ProductId = request.ProductForEditDto.ProductId;
                productImage.ImageId = request.ProductForEditDto.ProductImage;
                _db.ProductImages.Add(productImage);
            }

            if (request?.ProductForEditDto.ProductVariant != null)
            {
                foreach (var item in request.ProductForEditDto.ProductVariant)
                {
                    VariantImage variantImage = new VariantImage();
                    var varImgRemove = await _db.VariantImages.Where(o => o.VariantId == item.Id).ToListAsync(cancellationToken);
                    _db.VariantImages.RemoveRange(varImgRemove);

                    //var variantId = Guid.NewGuid().ToString();

                    var isVariant = await _db.Variants.Where(o => o.Id == item.Id).FirstOrDefaultAsync(cancellationToken);
                    if (isVariant != null)
                    {
                        isVariant.ProductId = request.ProductForEditDto.ProductId;
                        isVariant.Title = item.Title;
                        isVariant.SizeId = item.SizeId;
                        isVariant.ColorId = item.ColorId;
                        isVariant.Price = item.Price;
                        isVariant.Sku = item.Sku;
                        isVariant.Qty = item.Qty;

                        _db.Variants.Update(isVariant);

                        if (item?.VariantImageId != null)
                        {
                            variantImage.VariantId = item.Id;
                            variantImage.ImageId = item.VariantImageId;
                            await _db.VariantImages.AddAsync(variantImage, cancellationToken);
                        }
                    }
                    else
                    {
                        Variant variant = new Variant();
                        variant.ProductId = request.ProductForEditDto.ProductId;
                        variant.Title = item.Title;
                        variant.SizeId = item.SizeId;
                        variant.ColorId = item.ColorId;
                        variant.Price = item.Price;
                        variant.Sku = item.Sku;
                        variant.Qty = item.Qty;

                        await _db.Variants.AddAsync(variant, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);

                        if (item?.VariantImageId != null)
                        {
                            variantImage.VariantId = variant.Id;
                            variantImage.ImageId = item.VariantImageId;
                            _db.VariantImages.Add(variantImage);
                        }
                    }
                }
            }
            else
            {
                var allVariant = await _db.Variants.Where(o => o.ProductId == request.ProductForEditDto.ProductId).ToListAsync(cancellationToken);
                _db.Variants.RemoveRange(allVariant);
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(product.Name, "Product Successfully Updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to Update Product");
        }
    }
}
