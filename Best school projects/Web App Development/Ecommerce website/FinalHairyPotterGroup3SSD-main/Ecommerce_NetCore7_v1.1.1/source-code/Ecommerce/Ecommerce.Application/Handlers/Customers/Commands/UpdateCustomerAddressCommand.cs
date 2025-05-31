using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Commands;

public class UpdateCustomerAddressCommand : IRequest<Response<string>>
{
    public CustomerDto Customer { get; set; }
}
public class UpdateCustomerAddressCommandHandler : IRequestHandler<UpdateCustomerAddressCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly ICurrentUser _currentUser;
    public UpdateCustomerAddressCommandHandler(IDataContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Response<string>> Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Include(c=>c.User).FirstOrDefaultAsync(o => o.ApplicationUserId == _currentUser.UserId, cancellationToken);
        if (customer == null) return Response<string>.Fail("Sorry! No Customer Found to Update.");

        var timeNow = DateTime.UtcNow;
        customer.BillingAddress = request.Customer.BillingAddress;
        customer.ShippingAddress = request.Customer.ShippingAddress;
        customer.LastModifiedBy = _currentUser.UserName;
        customer.LastModifiedDate = timeNow;
        _db.Customers.Update(customer);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
