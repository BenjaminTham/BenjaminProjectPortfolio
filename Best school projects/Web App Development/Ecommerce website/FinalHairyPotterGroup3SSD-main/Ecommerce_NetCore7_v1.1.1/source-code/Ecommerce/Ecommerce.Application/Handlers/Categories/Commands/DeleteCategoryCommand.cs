using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.Categories.Commands;

public class DeleteCategoryCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteCategoryCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _db.Categories.Include(o => o.Children).Where(o => o.Id == request.Id).FirstOrDefaultAsync(); ;
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(category.Name, "Successfully created");
        }
        catch (Exception e)
        {
            return Response<string>.Fail(e.Message);
        }
    }
}
