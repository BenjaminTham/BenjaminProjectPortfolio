using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Orders.Queries;

//public class GetOrderByUserIdQuery : IRequest<List<OrderDto>>
//{
//    public string UserId { get; set; }
//}
//public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrderByUserIdQuery, List<OrderDto>>
//{
//    private readonly IDataContext _db;
//    private readonly IMapper _mapper;
//    public GetOrderByUserIdQueryHandler(IDataContext db, IMapper mapper)
//    {
//        _db = db;
//        _mapper = mapper;
//    }

//    public async Task<List<OrderDto>> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
//    {
//        if (request?.UserId == null) throw new Exception("CustomerId is null.");
//        var order = await _db.Orders.Where(o => o.CustomerId == request.UserId).ToListAsync(cancellationToken);
//        var result = _mapper.Map<List<OrderDto>>(order);
//        return result;
//    }
//}
