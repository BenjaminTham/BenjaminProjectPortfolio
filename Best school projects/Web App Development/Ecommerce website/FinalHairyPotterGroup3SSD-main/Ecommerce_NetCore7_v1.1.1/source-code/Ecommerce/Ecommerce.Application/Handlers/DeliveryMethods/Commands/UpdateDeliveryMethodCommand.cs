using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.DeliveryMethods.Commands;

public class UpdateDeliveryMethodCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}
public class UpdateDeliveryMethodCommandHandler : IRequestHandler<UpdateDeliveryMethodCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateDeliveryMethodCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateDeliveryMethodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var DeliveryMethod = await _db.DeliveryMethods.FindAsync(request.Id);
            _mapper.Map(request, DeliveryMethod);
            var updateDeliveryMethod = _db.DeliveryMethods.Update(DeliveryMethod);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(DeliveryMethod.Name, "Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
