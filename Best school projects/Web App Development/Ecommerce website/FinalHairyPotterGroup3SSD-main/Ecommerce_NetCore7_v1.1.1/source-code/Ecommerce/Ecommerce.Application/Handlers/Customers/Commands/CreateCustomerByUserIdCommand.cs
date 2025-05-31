using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Customers.Commands;

public class CreateCustomerByUserIdCommand : IRequest<Response<string>>
{
    public string UserId { get; set; }
}
public class CreateCustomerByUserIdCommandHandler : IRequestHandler<CreateCustomerByUserIdCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    public CreateCustomerByUserIdCommandHandler(IDataContext db, IMapper mapper, ICurrentUser currentUser, IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _mediator = mediator;
        _userManager = userManager;
    }
    public async Task<Response<string>> Handle(CreateCustomerByUserIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) return Response<string>.Fail("Sorry! No Registered User Found to Add Order!");

        var customer = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(o => o.ApplicationUserId == request.UserId, cancellationToken) ?? new Customer();
        if (customer?.Id > 0) return Response<string>.Fail("A Customer Already Exist Against this User!");

        var timeNow = DateTime.UtcNow;
        user.LastModifiedDate = timeNow;
        user.LastModifiedBy = _currentUser.UserName;
        customer.User = user;

        await using var ts = _db.BeginTransaction();
        await _userManager.AddToRoleAsync(user, DefaultApplicationRoles.Customer); // add role to the user
        _db.Customers.Update(customer);

        try
        {
            await _db.SaveChangesAsync(cancellationToken); // save changes to the database
            await ts.CommitAsync(cancellationToken);
            return Response<string>.Success("Successfully created");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add item!");
        }
    }
}