using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Application.Common;
using Ecommerce.Application.Helpers;
using MediatR;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Application.Dto;

namespace Ecommerce.Application.Handlers.Customers.Queries;


public class GetCustomersWithPagingQuery : IRequest<PaginatedList<CustomerDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetCustomersWithPagingQueryHandler : IRequestHandler<GetCustomersWithPagingQuery, PaginatedList<CustomerDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCustomersWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CustomerDto>> Handle(GetCustomersWithPagingQuery request, CancellationToken cancellationToken)
    {
        var customers = _db.Customers.Include(c=>c.User).OrderByDescending(o => o.LastModifiedDate).AsQueryable();
        var getCustomers =
            customers
                .Where(c => c.User.FirstName.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}")
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider).AsQueryable();

        var data = await PaginatedList<CustomerDto>.CreateAsync(getCustomers, request.page ?? 1, request.length);
        return data;
    }
}
