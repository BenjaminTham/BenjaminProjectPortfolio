using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Handlers.Categories.Commands;

public class CreateCategoryCommand : IRequest<Response<string>>
{
    [Required(ErrorMessage = "Category name is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_]*$", 
        ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '_' are allowed.")]
    public string Name { get; set; }

    public long? ParentCategoryId { get; set; }

    [Required(ErrorMessage = "Slug is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_-]*$", 
        ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '_', and '-' are allowed.")]
    public string Slug { get; set; }
}
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = _db.Categories.FirstOrDefault(c => c.Name == request.Name || c.Slug == request.Slug);

        if (existingCategory != null)
        {
            var errorMsg = "This Category Related Data Already Exist. Please Change those Data.";
            if (existingCategory.Name == request.Name) errorMsg += $" Name:[{existingCategory.Name}],";
            if (existingCategory.Slug == request.Slug) errorMsg += $" Slug:[{existingCategory.Slug}]";
            return Response<string>.Fail(errorMsg);
        }

        try
        {
            var category = _mapper.Map<Category>(request);
            var addcategory = await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(category.Name, "Successfully created");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add item!");
        }
    }
}
