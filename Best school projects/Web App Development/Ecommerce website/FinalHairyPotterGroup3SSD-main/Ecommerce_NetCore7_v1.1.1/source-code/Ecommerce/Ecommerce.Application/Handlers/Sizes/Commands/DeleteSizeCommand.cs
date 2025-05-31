using AutoMapper;
using Ecommerce.Application.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.Sizes.Commands;

public class DeleteSizeCommand : IRequest<Unit>
{
    public int Id { get; set; }
}

public class DeleteSizeCommandHandler : IRequestHandler<DeleteSizeCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteSizeCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteSizeCommand request, CancellationToken cancellationToken)
    {
        var size = await _db.Sizes.FindAsync(request.Id);
        _db.Sizes.Remove(size);
        await _db.SaveChangesAsync(cancellationToken);
        //var sizedto = _mapper.Map<SizeDto>(size);
        return Unit.Value;
    }
}
