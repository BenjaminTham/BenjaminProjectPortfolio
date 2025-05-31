using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Queries;

public class GetCustomerByUserIdQuery : IRequest<CustomerDto>
{
    public string Id { get; set; }
}
public class GetCustomerByUserIdQueryHandler : IRequestHandler<GetCustomerByUserIdQuery, CustomerDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCustomerByUserIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(GetCustomerByUserIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == request.Id);
        var result = _mapper.Map<CustomerDto>(customer);
        return result;
    }
}
