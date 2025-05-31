using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Inventory.Queries;

public class GetProductStockByIdQuery : IRequest<ProductStockDto>
{
    public long ProductId { get; set; }
}
public class GetProductStockByIdQueryHandler : IRequestHandler<GetProductStockByIdQuery, ProductStockDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductStockByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductStockDto> Handle(GetProductStockByIdQuery request, CancellationToken cancellationToken)
    {
        List<ProductVariantStockDto> productVariantStock = await (from pv in _db.Variants
                                                                  where pv.ProductId == request.ProductId
                                                                  join vi in _db.VariantImages on pv.Id equals vi.VariantId into vilist
                                                                  from vi in vilist.DefaultIfEmpty()
                                                                  join i in _db.Galleries on vi.ImageId equals i.Id into ilist
                                                                  from i in ilist.DefaultIfEmpty()

                                                                  select new ProductVariantStockDto
                                                                  {
                                                                      Id = pv.Id,
                                                                      Title = pv.Title,
                                                                      ProductId = pv.ProductId,
                                                                      Sku = pv.Sku,
                                                                      Price = pv.Price,
                                                                      Qty = pv.Qty,
                                                                      VariantImagePreview = i.Name == null ? null : i.Name
                                                                  }).ToListAsync();

        ProductStockDto productStock = await (from p in _db.Products.Include(c=>c.Category)
                                              where p.Id == request.ProductId
                                              join pi in _db.ProductImages on p.Id equals pi.ProductId into plist
                                              from pi in plist.DefaultIfEmpty()
                                              join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                                              from i in ilist.DefaultIfEmpty()

                                              select new ProductStockDto
                                              {
                                                  ProductId = p.Id,
                                                  CategoryName = p.Category.Name,
                                                  Name = p.Name,
                                                  ProductImagePreview = i.Name,
                                                  ProductVariant = productVariantStock.Count != 0 ? productVariantStock : null
                                              }).FirstOrDefaultAsync();

        return productStock;
    }
}
