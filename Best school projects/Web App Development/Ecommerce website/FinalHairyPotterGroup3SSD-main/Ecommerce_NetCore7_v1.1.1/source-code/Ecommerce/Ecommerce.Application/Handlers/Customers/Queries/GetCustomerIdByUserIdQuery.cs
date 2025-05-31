using AutoMapper;
using Ecommerce.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Queries;

public class GetCustomerIdByUserIdQuery : IRequest<long>
{
    public string Id { get; set; }
}
public class GetCustomerIdByUserIdQueryHandler : IRequestHandler<GetCustomerIdByUserIdQuery, long>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCustomerIdByUserIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<long> Handle(GetCustomerIdByUserIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Where(c => c.ApplicationUserId == request.Id).Select(c => c.Id).FirstOrDefaultAsync(cancellationToken);
        return customer;
    }
}
