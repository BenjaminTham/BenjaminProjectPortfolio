using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Queries;

public class GetCustomerByIdQuery : IRequest<CustomerDto>
{
    public long Id { get; set; }
}
public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCustomerByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Include(c=>c.User).Where(c=>c.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        var result = _mapper.Map<CustomerDto>(customer);
        return result;
    }
}
