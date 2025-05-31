using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Handlers.Colors.Commands;

public class CreateColorCommand : IRequest<Response<string>>
{
    [Required(ErrorMessage = "Color name is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_\-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', and '-' are allowed.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Hex code is required.")]
    [RegularExpression(@"^#([a-fA-F0-9]{6})$", ErrorMessage = "Invalid hex color code. It should be # followed by exactly six hexadecimal digits (0-9, A-F).")]
    public string HexCode { get; set; }

    public bool IsActive { get; set; }
}
public class CreateColorCommandHandler : IRequestHandler<CreateColorCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public CreateColorCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(CreateColorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var color = _mapper.Map<Color>(request);
            var addcolor = await _db.Colors.AddAsync(color);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(color.Name, "Successfully created");
        }
        //catch (ValidationException e)
        //{
        //    Console.WriteLine(e);
        //    return Response<string>.Fail("Failed to add item!");
        //}
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add item!");
        }
    }
}
