using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Application.Handlers.Inventory.Commands;

public class AddProductStockCommand : IRequest<Response<string>>
{
    public long VariantId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', and '_' are allowed.")]
    public string? Description { get; set; }


    [Required(ErrorMessage = "Quantity is required.")]
    [RegularExpression(@"^-?\d+$", ErrorMessage = "Quantity must be an integer.")]
    public int Qty { get; set; }
}
public class UpdateProductStockCommandHandler : IRequestHandler<AddProductStockCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public UpdateProductStockCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(AddProductStockCommand request, CancellationToken cancellationToken)
    {
        if (request.Qty < 1) return Response<string>.Fail($"Qty [{request.Qty}] is Not Valid. Please Add at Least 1 Item.");
        var getItem = await _db.Variants.FindAsync(request.VariantId);
        if(getItem == null) return Response<string>.Fail("Sorry! No Product Found to Add Stock.");
        getItem.Qty += request.Qty;
        _db.Variants.Update(getItem);

        Stock stock = new Stock()
        {
            VariantId = getItem.Id,
            StockInputType = StockInputType.Addition,
            Qty = request.Qty,
            Description = request.Description,
        };


        await using var transaction = _db.BeginTransaction();
        try
        {
            _db.Stocks.Add(stock);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Response<string>.Success(getItem.Qty.ToString(), "Successfully updated the category");
        }
        catch (System.Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Response<string>.Fail("Failed to add the category");
        }

    }
}
