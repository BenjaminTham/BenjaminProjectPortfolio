using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.OrderStatusValues.Queries;

public class GetOrderStatusValueWithPagingQuery : IRequest<PaginatedList<OrderStatusValueDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetOrderStatusValueWithPagingQueryHandler : IRequestHandler<GetOrderStatusValueWithPagingQuery, PaginatedList<OrderStatusValueDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderStatusValueWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OrderStatusValueDto>> Handle(GetOrderStatusValueWithPagingQuery request, CancellationToken cancellationToken)
    {
        var orderStatusValue = _db.OrderStatusValues.OrderByDescending(o => o.LastModifiedDate).AsQueryable();
        var getOrderStatusValue =
                orderStatusValue
                .Where(a => a.StatusValue.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .ProjectTo<OrderStatusValueDto>(_mapper.ConfigurationProvider);

        var data = await PaginatedList<OrderStatusValueDto>.CreateAsync(getOrderStatusValue, request.page ?? 1, request.length);
        return data;
    }
}
