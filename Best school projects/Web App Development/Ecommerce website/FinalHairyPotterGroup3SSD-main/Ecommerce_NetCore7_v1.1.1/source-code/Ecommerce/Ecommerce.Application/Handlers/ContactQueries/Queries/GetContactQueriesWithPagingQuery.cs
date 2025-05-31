using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.ContactQueries.Queries;


public class GetContactQueriesWithPagingQuery : IRequest<PaginatedList<ContactQueryDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetContactQueriesWithPagingQueryHandler : IRequestHandler<GetContactQueriesWithPagingQuery, PaginatedList<ContactQueryDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetContactQueriesWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ContactQueryDto>> Handle(GetContactQueriesWithPagingQuery request, CancellationToken cancellationToken)
    {
        var contactQueries = _db.ContactQueries.OrderByDescending(o => o.LastModifiedDate).AsQueryable();
        var getContactQueries =
            contactQueries
                .Where(a => a.FullName.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .ProjectTo<ContactQueryDto>(_mapper.ConfigurationProvider);

        var data = await PaginatedList<ContactQueryDto>.CreateAsync(getContactQueries, request.page ?? 1, request.length);
        return data;
    }
}
