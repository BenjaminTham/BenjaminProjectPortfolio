using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Identity.Constants;
using Ecommerce.Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.CustomerAccount.Commands;

public class RegisterCustomerCommand : IRequest<Response<UserIdentityDto>>
{
    public CustomerRegisterDto CustomerRegister { get; set; }
}
public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, Response<UserIdentityDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IKeyAccessor _keyAccessor;
    private readonly ICurrentUser _currentUser;
    public RegisterCustomerCommandHandler(IDataContext db, IMapper mapper, UserManager<ApplicationUser> userManager, IKeyAccessor keyAccessor, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _userManager = userManager;
        _keyAccessor = keyAccessor;
        _currentUser = currentUser;
    }

    public async Task<Response<UserIdentityDto>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var timeNow = DateTime.UtcNow;

        await using var ts = _db.BeginTransaction();

        try
        {
            var user = new ApplicationUser
            {
                UserName = request.CustomerRegister.UserName,
                Email = request.CustomerRegister.Email,
                FirstName = request.CustomerRegister.FirstName,
                LastName = request.CustomerRegister.LastName,
                CreatedBy = _currentUser.UserName,
                CreatedDate = timeNow,
                LastModifiedBy = _currentUser.UserName,
                LastModifiedDate = timeNow,
                Gender = Gender.Unknown.ToString(),
                IsActive = true,
                //This is how we will add customer data if needed
                Customer = new Customer
                {
                    
                }
            };

            var userAdded = await _userManager.CreateAsync(user, request.CustomerRegister.Password);
            if (userAdded.Succeeded)
            {
                await _db.SaveChangesAsync(cancellationToken); // save changes to the database
                await _userManager.AddToRoleAsync(user, DefaultApplicationRoles.Customer); // add role to the user
                await ts.CommitAsync(cancellationToken);
                return Response<UserIdentityDto>.Success(new UserIdentityDto { Id = user.Id }, userAdded.ToString());
            }
        }
        catch (Exception ex)
        {
            await ts.RollbackAsync(cancellationToken);
            return Response<UserIdentityDto>.Fail("An error occurred!");
        }
        await ts.RollbackAsync(cancellationToken);
        return Response<UserIdentityDto>.Fail("An error occurred!");
    }

}
