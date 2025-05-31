using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Commands;

public class UpdateCustomerCommand : IRequest<Response<string>>
{
    public long Id { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public string? UserPhoneNumber { get; set; }
    public string UserEmail { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
}
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateCustomerCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Include(c => c.User).Where(c => c.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (customer != null)
        {
            _mapper.Map(request, customer);
            customer.User.FirstName = request.UserFirstName;
            customer.User.LastName = request.UserLastName;
            customer.User.PhoneNumber = request.UserPhoneNumber;
            customer.User.Email = request.UserEmail;

            var updateCustomer = _db.Customers.Update(customer);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(customer.User.FullName, "Successfully Updated");
        }
        return Response<string>.Fail("Failed To Update");
    }
}
