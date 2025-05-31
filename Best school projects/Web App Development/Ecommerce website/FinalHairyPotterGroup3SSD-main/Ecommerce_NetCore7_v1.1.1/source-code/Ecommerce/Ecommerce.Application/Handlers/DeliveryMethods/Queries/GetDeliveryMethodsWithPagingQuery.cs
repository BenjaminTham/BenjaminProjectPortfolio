using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.DeliveryMethods.Queries;

public class GetDeliveryMethodsWithPagingQuery : IRequest<PaginatedList<DeliveryMethodDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetDeliveryMethodsWithPagingQueryHandler : IRequestHandler<GetDeliveryMethodsWithPagingQuery, PaginatedList<DeliveryMethodDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetDeliveryMethodsWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DeliveryMethodDto>> Handle(GetDeliveryMethodsWithPagingQuery request, CancellationToken cancellationToken)
    {
        var deliveryMethods = _db.DeliveryMethods.OrderByDescending(o => o.LastModifiedDate).AsQueryable();
        var getDeliveryMethods =
                deliveryMethods
                .Where(a => a.Name.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .ProjectTo<DeliveryMethodDto>(_mapper.ConfigurationProvider);

        var data = await PaginatedList<DeliveryMethodDto>.CreateAsync(getDeliveryMethods, request.page ?? 1, request.length);
        return data;
    }
}
