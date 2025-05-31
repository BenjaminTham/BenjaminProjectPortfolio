using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.Categories.Commands;

public class UpdateCategoryCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int? ParentCategoryId { get; set; }
}
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateCategoryCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _db.Categories.FindAsync(request.Id);
            _mapper.Map(request, category);
            _db.Categories.Update(category);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(category.Name, "Successfully updated the category");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to add the category");
        }
    }
}
