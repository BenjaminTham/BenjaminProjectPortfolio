using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Shop.Queries;

public class GetProductSizesByColorQuery : IRequest<List<ProductSizesByColorFilterResultDto>>
{
    public int Id { get; set; }
    public string Color { get; set; }
}
public class GetProductSizesByColorQueryHandler : IRequestHandler<GetProductSizesByColorQuery, List<ProductSizesByColorFilterResultDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetProductSizesByColorQueryHandler(IDataContext db, IKeyAccessor keyAccessor, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<List<ProductSizesByColorFilterResultDto>> Handle(GetProductSizesByColorQuery request, CancellationToken cancellationToken)
    {
        StockConfiguration conStock = JsonSerializer.Deserialize<StockConfiguration>(_keyAccessor.GetSection("StockConfiguration"))!;

        var filterResult = await (from pv in _db.Variants
                                  where pv.ProductId == request.Id && pv.ColorId == Int32.Parse(request.Color)
                                  join vi in _db.VariantImages on pv.Id equals vi.VariantId into vilist
                                  from vi in vilist.DefaultIfEmpty()
                                  join i in _db.Galleries on vi.ImageId equals i.Id into ilist
                                  from i in ilist.DefaultIfEmpty()
                                  join s in _db.Sizes on pv.SizeId equals s.Id into slist
                                  from s in slist.DefaultIfEmpty()
                                  where (conStock.IsOutOfStockItemHidden != true || (pv.Qty > conStock.OutOfStockThreshold))
                                  select new ProductSizesByColorFilterResultDto
                                  {
                                      SizeId = s.Id,
                                      Name = s.Name
                                  }).OrderBy(o => o.SizeId).ToListAsync(cancellationToken);

        return filterResult;
    }
}
