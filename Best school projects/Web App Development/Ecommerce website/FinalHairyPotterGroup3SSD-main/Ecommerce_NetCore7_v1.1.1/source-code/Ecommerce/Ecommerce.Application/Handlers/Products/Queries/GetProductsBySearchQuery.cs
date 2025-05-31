using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Products.Queries;

public class GetProductsBySearchQuery : IRequest<IEnumerable<ProductLiveSearchDto>>
{
    [Required]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Search value can only contain letters and spaces.")]
    public string SearchValue { get; set; }
    public int MaxResult { get; set; } = 10;
}
public class GetProductsBySearchQueryHandler : IRequestHandler<GetProductsBySearchQuery, IEnumerable<ProductLiveSearchDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetProductsBySearchQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductLiveSearchDto>> Handle(GetProductsBySearchQuery request, CancellationToken cancellationToken)
    {
        var product = (from p in _db.Products.Include(o => o.Category)
                  .Where(o => o.Name.ToLower().Contains(request.SearchValue))
                  .Take(request.MaxResult)
                       join pi in _db.ProductImages on p.Id equals pi.ProductId into plist
                       from pi in plist.DefaultIfEmpty()
                       join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                       from i in ilist.DefaultIfEmpty()
                       select new ProductLiveSearchDto
                       {
                           ProductId = p.Id,
                           ProductName = p.Name,
                           ProductSlug = p.Slug,
                           ProductCategory = p.Category.Name,
                           ImagePreview = i.Name
                       }).AsQueryable();

        return await product.ToListAsync();
    }
}
