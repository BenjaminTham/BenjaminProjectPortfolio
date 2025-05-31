using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.Colors.Queries;

public class GetColorsWithPagingQuery : IRequest<PaginatedList<ColorDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetColorsWithPagingQueryHandler : IRequestHandler<GetColorsWithPagingQuery, PaginatedList<ColorDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetColorsWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ColorDto>> Handle(GetColorsWithPagingQuery request, CancellationToken cancellationToken)
    {
        var colors = _db.Colors.OrderByDescending(o => o.LastModifiedDate).AsQueryable();
        var getcolors =
                colors
                .Where(a => a.Name.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .ProjectTo<ColorDto>(_mapper.ConfigurationProvider);

        var data = await PaginatedList<ColorDto>.CreateAsync(getcolors, request.page ?? 1, request.length);
        return data;
    }
}
