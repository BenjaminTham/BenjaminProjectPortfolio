using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Handlers.Sizes.Commands;

public class CreateSizeCommand : IRequest<Response<string>>
{

    [Required(ErrorMessage = "Size name is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s-]*$", ErrorMessage = "Only alphanumeric characters, spaces, and '-' are allowed.")]
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
public class CreateSizeCommandHandler : IRequestHandler<CreateSizeCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public CreateSizeCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(CreateSizeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var size = _mapper.Map<Size>(request);
            var addsize = await _db.Sizes.AddAsync(size);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(size.Name, "Successfully created");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add item!");
        }
    }
}
