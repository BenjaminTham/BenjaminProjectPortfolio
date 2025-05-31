using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.OrderStatusValues.Queries;

public class GetOrderStatusValueByIdQuery : IRequest<OrderStatusValueDto>
{
    public int Id { get; set; }
}
public class GetOrderStatusValueByIdQueryHandler : IRequestHandler<GetOrderStatusValueByIdQuery, OrderStatusValueDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetOrderStatusValueByIdQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<OrderStatusValueDto> Handle(GetOrderStatusValueByIdQuery request, CancellationToken cancellationToken)
    {
        var orderStatusValue = await _db.OrderStatusValues.FindAsync(request.Id);
        var result = _mapper.Map<OrderStatusValueDto>(orderStatusValue);
        return result;
    }
}
