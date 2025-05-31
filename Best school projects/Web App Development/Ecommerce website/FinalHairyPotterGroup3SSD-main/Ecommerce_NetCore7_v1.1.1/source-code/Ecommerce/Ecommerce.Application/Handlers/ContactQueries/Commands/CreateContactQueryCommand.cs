using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Handlers.ContactQueries.Commands;

public class CreateContactQueryCommand : IRequest<Response<string>>
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Subject { get; set; }
    public string MessageBody { get; set; }
}
public class CreateContactQueryCommandHandler : IRequestHandler<CreateContactQueryCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public CreateContactQueryCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(CreateContactQueryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contactQuery = _mapper.Map<ContactQuery>(request);
            var addContactQuery = await _db.ContactQueries.AddAsync(contactQuery);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(contactQuery.FullName, "Message Send Successfully!");
        }
        //catch (ValidationException e)
        //{
        //    Console.WriteLine(e);
        //    return Response<string>.Fail("Failed to add item!");
        //}
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to Send Message!");
        }
    }
}
