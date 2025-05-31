using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.Colors.Commands;

public class DeleteColorCommand : IRequest<Unit>
{
    public int Id { get; set; }
}

public class DeleteColorCommandHandler : IRequestHandler<DeleteColorCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteColorCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteColorCommand request, CancellationToken cancellationToken)
    {
        var color = await _db.Colors.FindAsync(request.Id);
        _db.Colors.Remove(color);
        await _db.SaveChangesAsync(cancellationToken);
        var colordto = _mapper.Map<ColorDto>(color);
        return Unit.Value;
    }
}
