using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.Sizes.Commands;

public class UpdateSizeCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
public class UpdateSizeCommandHandler : IRequestHandler<UpdateSizeCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateSizeCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateSizeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var size = await _db.Sizes.FindAsync(request.Id);
            _mapper.Map(request, size);
            var updatesize = _db.Sizes.Update(size);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(size.Name, "Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
