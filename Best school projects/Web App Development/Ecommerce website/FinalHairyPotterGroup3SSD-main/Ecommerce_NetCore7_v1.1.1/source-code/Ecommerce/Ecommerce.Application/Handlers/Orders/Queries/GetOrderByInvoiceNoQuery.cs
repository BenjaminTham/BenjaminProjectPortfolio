using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetOrderByInvoiceNoQuery : IRequest<OrderDto>
{
    public string InvoiceNo { get; set; }
}
public class GetOrderByInvoiceNoQueryHandler : IRequestHandler<GetOrderByInvoiceNoQuery, OrderDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderByInvoiceNoQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByInvoiceNoQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.Where(o => o.InvoiceNo == request.InvoiceNo).FirstOrDefaultAsync();
        var result = _mapper.Map<OrderDto>(order);
        return result;
    }
}
