using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Products.Queries;

public class GetProductVariablesByIdQuery : IRequest<ProductVariantForEditDto>
{
    public int Id { get; set; }
}
public class GetProductVariablesByIdQueryHandler : IRequestHandler<GetProductVariablesByIdQuery, ProductVariantForEditDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductVariablesByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductVariantForEditDto> Handle(GetProductVariablesByIdQuery request, CancellationToken cancellationToken)
    {
        var productVariant = await (from p in _db.Products.Include(o => o.Category)
                             join pv in _db.Variants on p.Id equals pv.ProductId into pvlist
                             from pv in pvlist.DefaultIfEmpty()
                             join vi in _db.VariantImages on pv.Id equals vi.VariantId into vilist
                             from vi in vilist.DefaultIfEmpty()
                             join pi in _db.ProductImages on p.Id equals pi.ProductId into plist
                             from pi in plist.DefaultIfEmpty()
                             join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                             from i in ilist.DefaultIfEmpty()
                             where pv.Id == request.Id
                             select new ProductVariantForEditDto
                             {
                                 Id = pv.Id,
                                 CategoryName = p.Category.Name,
                                 Title = pv.Title,
                                 Slug = p.Slug,
                                 ProductId = pv.ProductId,
                                 Price = pv.Price,
                                 Qty = pv.Qty,
                                 VariantImageId = vi.ImageId,
                                 VariantImagePreview = i.Name
                             }).FirstOrDefaultAsync(cancellationToken);

        return productVariant;
    }
}
