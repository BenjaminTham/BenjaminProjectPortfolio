using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Handlers.Products.Commands;

public class CreateProductCommand : IRequest<Response<string>>
{
    [Required(ErrorMessage = "Product name is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '_' are allowed in the product name.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Slug is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s-_]*$", ErrorMessage = "Only alphanumeric characters, spaces, '-', and '_' are allowed in the slug.")]
    public string Slug { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_'-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '-', and '_' are allowed in the short description.")]
    public string? ShortDescription { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_'-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '-', and '_' are allowed in the description.")]
    public string? Description { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_'-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '-', and '_' are allowed in the variable theme.")]
    public string? VariableTheme { get; set; }

    [Required(ErrorMessage = "Category ID is required.")]
    public int CategoryId { get; set; }
}
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public CreateProductCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = _mapper.Map<Product>(request);
            var addproduct = await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(product.Name, "Successfully added the product");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add the product");
        }
    }
}
