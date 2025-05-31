using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Cart.Commands;

public class AddToCartCommand : IRequest<Unit>
{
    public long VariantId { get; set; }
    public int Qty { get; set; }
}
public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Unit>
{
    private readonly IDataContext _db;
    private readonly ICookieService _cookie;
    private readonly IMapper _mapper;
    public AddToCartCommandHandler(IDataContext db, IMapper mapper, ICookieService cookie)
    {
        _db = db;
        _mapper = mapper;
        _cookie = cookie;
    }

    public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        List<CartDto> cart = new List<CartDto>();
        if (_cookie.Contains("shop-cart"))
        {
            cart = JsonSerializer.Deserialize<List<CartDto>>(_cookie.Get("shop-cart")) ?? new List<CartDto>();
        }


        if (cart.Any(o => o.VariantId == request.VariantId))
        {
            var singleCartItem = cart.FirstOrDefault(o => o.VariantId == request.VariantId);
            int index = cart.IndexOf(singleCartItem);
            singleCartItem.Qty += request.Qty;
            cart.Remove(singleCartItem);
            cart.Insert(index, singleCartItem);
        }
        else
        {
            var item = await (from pv in _db.Variants
                              join vi in _db.VariantImages on pv.Id equals vi.VariantId into viList
                              from vi in viList.DefaultIfEmpty()
                              join i in _db.Galleries on vi.ImageId equals i.Id into iList
                              from i in iList.DefaultIfEmpty()
                              where pv.Id == request.VariantId
                              select new CartDto
                              {
                                  ProductId = pv.ProductId,
                                  VariantId = pv.Id,
                                  Title = pv.Title,
                                  Price = pv.Price,
                                  Qty = pv.Qty,
                                  Sku = pv.Sku,
                                  Image = i.Name
                              }).FirstOrDefaultAsync(cancellationToken);

            CartDto cartVm = new CartDto();
            cartVm.ProductId = item.ProductId;
            cartVm.VariantId = item.VariantId;
            cartVm.Title = item.Title;
            cartVm.Price = item.Price;
            cartVm.Qty = request.Qty;
            cartVm.StockQty = item.Qty;
            cartVm.Image = item.Image;
            cartVm.Sku = item.Sku;
            cart.Add(cartVm);
        }

        _cookie.Set("shop-cart", JsonSerializer.Serialize(cart), 24 * 60);
        return Unit.Value;
    }
}
