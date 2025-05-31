using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetOrderByInvoiceNoAndCustomerIdQuery : IRequest<OrderDto>
{
    public string InvoiceNo { get; set; }
    public long CustomerId { get; set; }
}
public class GetOrderByInvoiceNoAndCustomerIdQueryHandler : IRequestHandler<GetOrderByInvoiceNoAndCustomerIdQuery, OrderDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderByInvoiceNoAndCustomerIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByInvoiceNoAndCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.Where(o => o.InvoiceNo == request.InvoiceNo && o.CustomerId == request.CustomerId).FirstOrDefaultAsync(cancellationToken);
        var result = _mapper.Map<OrderDto>(order);
        return result;
    }
}
