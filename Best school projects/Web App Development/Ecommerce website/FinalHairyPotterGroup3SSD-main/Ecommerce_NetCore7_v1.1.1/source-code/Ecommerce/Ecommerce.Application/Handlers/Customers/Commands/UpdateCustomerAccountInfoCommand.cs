using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Application.Handlers.Customers.Commands;

public class UpdateCustomerAccountInfoCommand : IRequest<Response<string>>
{
    public CustomerDto Customer { get; set; }
}
public class UpdateCustomerAccountInfoCommandHandler : IRequestHandler<UpdateCustomerAccountInfoCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationUserManager _userManager;
    public UpdateCustomerAccountInfoCommandHandler(IDataContext db, ICurrentUser currentUser, IApplicationUserManager userManager)
    {
        _db = db;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<Response<string>> Handle(UpdateCustomerAccountInfoCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(o => o.ApplicationUserId == _currentUser.UserId, cancellationToken);
        if (customer == null) return Response<string>.Fail("Sorry! No Customer Found to Update.");

        var timeNow = DateTime.UtcNow;

        customer.User.FirstName = request.Customer.UserFirstName;
        customer.User.LastName = request.Customer.UserLastName;
        customer.User.Email = request.Customer.UserEmail;
        customer.User.PhoneNumber = request.Customer.UserPhoneNumber;
        customer.LastModifiedBy = _currentUser.UserName;
        customer.LastModifiedDate = timeNow;
        _db.Customers.Update(customer);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);

            //#region Update Claims
            //var user = customer.User;
            //var claims = await _userManager.GetClaimsAsync(user);
            //if (claims.Any())
            //{
            //    claims.Where(c => c.Type == "FullName");
            //    await _userManager.RemoveClaimsAsync(user, claims.ToList());
            //}
            //var fullName = new Claim("FullName", user.FullName.ToString());
            //await _userManager.AddClaimAsync(user, fullName);
            //var rs = await _userManager.UpdateAsync(user);
            //#endregion

            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
