using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetOrderInvoiceByOrderIdQuery : IRequest<OrderInvoiceDto>
{
    public int Id { get; set; }
}
public class GetOrderInvoiceByOrderIdQueryHandler : IRequestHandler<GetOrderInvoiceByOrderIdQuery, OrderInvoiceDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetOrderInvoiceByOrderIdQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<OrderInvoiceDto> Handle(GetOrderInvoiceByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var genConfig = JsonSerializer.Deserialize<GeneralConfigurationDto>(_keyAccessor.GetSection("GeneralConfiguration"));

        var order = await _db.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.Id == request.Id)
            .FirstOrDefaultAsync();


        var orderItems = order.OrderDetails
            .Select(o => new OrderInvoiceOrderItems
            {
                Item = o.ProductName,
                Quantity = o.Qty,
                UnitPrice = o.UnitPrice,
                Total = decimal.Round(o.UnitPrice * o.Qty, 2),
                UnitPriceWithCurrency = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + o.UnitPrice.ToString() : o.UnitPrice.ToString() + genConfig.CurrencySymbol,
                TotalWithCurrency = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + (decimal.Round(o.UnitPrice * o.Qty, 2)) : (decimal.Round(o.UnitPrice * o.Qty, 2)) + genConfig.CurrencySymbol
            })
            .ToList();

        var subTotal = order.OrderAmount;
        var deliveryCharge = order.DeliveryCharge;
        var total = order.OrderAmount + order.DeliveryCharge;


        var orderInvoice = new OrderInvoiceDto
        {
            OrderId = order.Id,
            InvoiceNo = order.InvoiceNo,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerMobile,
            ShippingAddress = order.ShippingAddress,
            OrderDate = order.CreatedDate.Value.ToString("dd-MM-yyyy"),
            PaymentMethod = order.PaymentMethod,
            Subtotal = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + subTotal.ToString() : subTotal.ToString() + genConfig.CurrencySymbol,
            DeliveryCharge = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + deliveryCharge.ToString() : deliveryCharge.ToString() + genConfig.CurrencySymbol,
            TotalAmount = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + total.ToString() : total.ToString() + genConfig.CurrencySymbol,
            OrderItems = orderItems,

        };

        return orderInvoice;
    }
}
