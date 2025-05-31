using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using MediatR;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Cart.Commands;

public class DecrementCartItemCommand : IRequest<Unit>
{
    public long VariantId { get; set; }
}
public class DecrementCartItemCommandHandler : IRequestHandler<DecrementCartItemCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly ICookieService _cookie;
    private readonly IMapper _mapper;
    public DecrementCartItemCommandHandler(IDataContext db, IMapper mapper, ICookieService cookie)
    {
        _db = db;
        _mapper = mapper;
        _cookie = cookie;
    }

    public async Task<Unit> Handle(DecrementCartItemCommand request, CancellationToken cancellationToken)
    {
        List<CartDto> cart = new List<CartDto>();
        if (_cookie.Contains("shop-cart"))
        {
            cart = JsonSerializer.Deserialize<List<CartDto>>(_cookie.Get("shop-cart"));
        }

        var singleCartItem = cart.Where(o => o.VariantId == request.VariantId).FirstOrDefault();
        int index = cart.IndexOf(singleCartItem);

        if (singleCartItem.Qty <= 1)
        {
            cart.Remove(singleCartItem);
        }
        else
        {
            singleCartItem.Qty -= 1;
            cart.Remove(singleCartItem);
            cart.Insert(index, singleCartItem);
        }

        _cookie.Set("shop-cart", JsonSerializer.Serialize(cart), 24 * 60);
        return await Task.FromResult(Unit.Value);
    }
}
