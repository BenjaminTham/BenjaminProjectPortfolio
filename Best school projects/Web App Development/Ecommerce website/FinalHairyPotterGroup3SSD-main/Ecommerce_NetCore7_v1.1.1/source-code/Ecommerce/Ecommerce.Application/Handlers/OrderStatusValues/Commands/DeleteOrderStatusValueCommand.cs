using AutoMapper;
using Ecommerce.Application.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.OrderStatusValues.Commands;

public class DeleteOrderStatusValueCommand : IRequest<Unit>
{
    public int Id { get; set; }
}

public class DeleteOrderStatusValueCommandHandler : IRequestHandler<DeleteOrderStatusValueCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteOrderStatusValueCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteOrderStatusValueCommand request, CancellationToken cancellationToken)
    {
        var orderStatusValue = await _db.OrderStatusValues.FindAsync(request.Id);
        _db.OrderStatusValues.Remove(orderStatusValue);
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
