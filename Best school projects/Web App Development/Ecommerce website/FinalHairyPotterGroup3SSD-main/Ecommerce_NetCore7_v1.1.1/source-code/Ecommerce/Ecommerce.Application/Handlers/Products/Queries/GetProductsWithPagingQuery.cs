using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Products.Queries;

public class GetProductsWithPagingQuery : IRequest<PaginatedList<ProductDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetProductsWithPagingQueryHandler : IRequestHandler<GetProductsWithPagingQuery, PaginatedList<ProductDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductsWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsWithPagingQuery request, CancellationToken cancellationToken)
    {
        var products = (from p in _db.Products.Include(o => o.Category)
                       join pi in _db.ProductImages on p.Id equals pi.ProductId into plist
                       from pi in plist.DefaultIfEmpty()
                       join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                       from i in ilist.DefaultIfEmpty()
                       select new ProductDto
                       {
                           Id = p.Id,
                           Name = p.Name,
                           CategoryName = p.Category.Name,
                           ImagePreview = i.Name
                       }).AsQueryable();

        products = products
                .Where(a => a.Name.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}");

        var data = await PaginatedList<ProductDto>.CreateAsync(products, request.page ?? 1, request.length);
        return data;
    }
}
