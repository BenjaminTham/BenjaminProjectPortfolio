using AutoMapper;
using Ecommerce.Application.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.DeliveryMethods.Commands;

public class DeleteDeliveryMethodCommand : IRequest<Unit>
{
    public int Id { get; set; }
}

public class DeleteDeliveryMethodCommandHandler : IRequestHandler<DeleteDeliveryMethodCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteDeliveryMethodCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteDeliveryMethodCommand request, CancellationToken cancellationToken)
    {
        var deliveryMethod = await _db.DeliveryMethods.FindAsync(request.Id);
        _db.DeliveryMethods.Remove(deliveryMethod);
        await _db.SaveChangesAsync(cancellationToken);
        //var DeliveryMethoddto = _mapper.Map<DeliveryMethodDto>(DeliveryMethod);
        return Unit.Value;
    }
}
