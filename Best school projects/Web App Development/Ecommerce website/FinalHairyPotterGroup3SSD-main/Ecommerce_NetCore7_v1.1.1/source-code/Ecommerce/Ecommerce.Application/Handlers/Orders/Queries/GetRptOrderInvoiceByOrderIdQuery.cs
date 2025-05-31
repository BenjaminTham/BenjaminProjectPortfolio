using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Models;
using Ecommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Orders.Queries;

public class GetRptOrderInvoiceByOrderIdQuery : IRequest<RptOrderInvoiceDto>
{
    public string InvoiceNo { get; set; }
}
public class GetRptOrderInvoiceByOrderIdQueryHandler : IRequestHandler<GetRptOrderInvoiceByOrderIdQuery, RptOrderInvoiceDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetRptOrderInvoiceByOrderIdQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<RptOrderInvoiceDto> Handle(GetRptOrderInvoiceByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var genConfig = JsonSerializer.Deserialize<GeneralConfigurationDto>(_keyAccessor.GetSection("GeneralConfiguration"));

        var order = await _db.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.InvoiceNo == request.InvoiceNo)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null) throw new Exception("No order found!");

        var orderItems = order.OrderDetails
            .Select(o => new RptOrderInvoiceOrderItems
            {
                OrderId = o.OrderId,
                Item = o.ProductName,
                Quantity = o.Qty,
                UnitPrice = o.UnitPrice,
                Total = decimal.Round(o.UnitPrice * o.Qty, 2),
                UnitPriceWithCurrency = genConfig?.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + o.UnitPrice.ToString() : o.UnitPrice.ToString() + genConfig?.CurrencySymbol,
                TotalWithCurrency = genConfig?.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + (decimal.Round(o.UnitPrice * o.Qty, 2)) : (decimal.Round(o.UnitPrice * o.Qty, 2)) + genConfig?.CurrencySymbol
            })
            .ToList();

        var subTotal = order.OrderAmount;
        var deliveryCharge = order.DeliveryCharge;
        var total = order.OrderAmount + order.DeliveryCharge;

        var paidAmount = await _db.OrderPayments.Where(c => c.OrderId == order.Id).Select(c => c.Amount).SumAsync(cancellationToken);

        List<RptOrderInvoicePageInfo> generalResource = new()
        {
            new RptOrderInvoicePageInfo { CompanyName = genConfig.CompanyName }
        };

        List<ReportParameter> customerInfo = new()
        {
            new ReportParameter { Name = "Customer", Value = order.CustomerName },
            new ReportParameter { Name = "Phone", Value = order.CustomerMobile },
            new ReportParameter { Name = "Address", Value = order.ShippingAddress }
        };

        List<ReportParameter> orderInfo = new()
        {
            new ReportParameter { Name = "Order/Invoice No", Value = order.InvoiceNo },
            new ReportParameter { Name = "Order Date", Value = order.CreatedDate.Value.ToString("ddd MMM yyyy") },
            new ReportParameter { Name = "Payment Method", Value = order.PaymentMethod },
            new ReportParameter { Name = "Paid Amount", Value = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + paidAmount.ToString() : paidAmount.ToString() + genConfig.CurrencySymbol }
        };

        List<ReportParameter> orderAmount = new()
        {
            new ReportParameter { Name = "Sub Total", Value = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + subTotal.ToString() : subTotal.ToString() + genConfig.CurrencySymbol },
            new ReportParameter { Name = "Delivery Charge", Value = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + deliveryCharge.ToString() : deliveryCharge.ToString() + genConfig.CurrencySymbol },
            new ReportParameter { Name = "Amount", Value = genConfig.CurrencyPosition == CurrencyPosition.Start ? genConfig.CurrencySymbol + total.ToString() : total.ToString() + genConfig.CurrencySymbol },
        };

        var orderInvoice = new RptOrderInvoiceDto
        {
            ReportPageInfos = generalResource,
            InvoiceNo = order.InvoiceNo,
            CustomerInfo = customerInfo,
            OrderSummary = orderInfo,
            OrderItems = orderItems,
            OrderAmount = orderAmount,
        };

        return orderInvoice;
    }
}
