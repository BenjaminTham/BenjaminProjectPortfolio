using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.Colors.Commands;

public class UpdateColorCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string HexCode { get; set; }
    public bool IsActive { get; set; }
}
public class UpdateColorCommandHandler : IRequestHandler<UpdateColorCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateColorCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(UpdateColorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var color = await _db.Colors.FindAsync(request.Id);
            _mapper.Map(request, color);
            _db.Colors.Update(color);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success(color.Name, "Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }
    }
}
