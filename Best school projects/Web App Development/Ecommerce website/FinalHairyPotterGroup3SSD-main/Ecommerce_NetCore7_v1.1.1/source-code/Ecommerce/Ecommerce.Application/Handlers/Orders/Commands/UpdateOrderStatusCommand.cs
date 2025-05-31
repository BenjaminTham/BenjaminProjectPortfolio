using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Handlers.Orders.Commands;

public class UpdateOrderStatusCommand : IRequest<Response<string>>
{
    public UpdateOrderStatusDto UpdateOrderStatus { get; set; }
}
public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateOrderStatusCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var orderStatus = new OrderStatus();
        orderStatus.OrderId = request.UpdateOrderStatus.OrderId;
        orderStatus.OrderStatusValueId = request.UpdateOrderStatus.NewOrderStatus;
        orderStatus.Description = request.UpdateOrderStatus.Description;

        try
        {
            _db.OrderStatus.Add(orderStatus);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
