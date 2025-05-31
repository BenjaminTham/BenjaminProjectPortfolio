using AutoMapper;
using Ecommerce.Application.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.Customers.Commands;

public class DeleteCustomerCommand : IRequest<Unit>
{
    public long Id { get; set; }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteCustomerCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.FindAsync(request.Id);
        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
