using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Products.Queries;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public long Id { get; set; }
}
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await (from p in _db.Products.Include(o => o.Category)
                       join pi in _db.ProductImages on p.Id equals pi.ProductId into plist
                       from pi in plist.DefaultIfEmpty()
                       join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                       from i in ilist.DefaultIfEmpty()
                       where p.Id == request.Id
                       select new ProductDto
                       {
                           Id = p.Id,
                           Name = p.Name,
                           Slug = p.Slug,
                           CategoryName = p.Category.Name,
                           ImagePreview = i.Name
                       }).FirstOrDefaultAsync(cancellationToken);

        return product;
    }
}
