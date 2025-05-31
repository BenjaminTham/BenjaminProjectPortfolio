using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Sizes.Queries;

public class GetSizesWithPagingQuery : IRequest<PaginatedList<SizeDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetSizesWithPagingQueryHandler : IRequestHandler<GetSizesWithPagingQuery, PaginatedList<SizeDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetSizesWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SizeDto>> Handle(GetSizesWithPagingQuery request, CancellationToken cancellationToken)
    {
        var sizes = _db.Sizes.OrderByDescending(o => o.LastModifiedDate).AsQueryable();
        var getsizes =
                sizes
                .Where(a => a.Name.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .ProjectTo<SizeDto>(_mapper.ConfigurationProvider);

        var data = await PaginatedList<SizeDto>.CreateAsync(getsizes, request.page ?? 1, request.length);
        return data;
    }
}
