using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.OrderStatusValues.Commands;

public class UpdateOrderStatusValueCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
    public string StatusValue { get; set; }
    public string Description { get; set; }
}
public class UpdateOrderStatusValueCommandHandler : IRequestHandler<UpdateOrderStatusValueCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateOrderStatusValueCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateOrderStatusValueCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var orderStatusValue = await _db.OrderStatusValues.FindAsync(request.Id);
            _mapper.Map(request, orderStatusValue);
            var updateOrderStatusValue = _db.OrderStatusValues.Update(orderStatusValue);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(orderStatusValue.StatusValue, "Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
