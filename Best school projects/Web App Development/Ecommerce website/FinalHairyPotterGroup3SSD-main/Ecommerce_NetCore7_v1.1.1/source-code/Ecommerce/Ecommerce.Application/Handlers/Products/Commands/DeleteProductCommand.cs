using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;

namespace Ecommerce.Application.Handlers.Products.Commands;

public class DeleteProductCommand : IRequest<ProductDto>
{
    public long Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public DeleteProductCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync(request.Id);
        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
        var productdto = _mapper.Map<ProductDto>(product);
        return productdto;
    }
}
