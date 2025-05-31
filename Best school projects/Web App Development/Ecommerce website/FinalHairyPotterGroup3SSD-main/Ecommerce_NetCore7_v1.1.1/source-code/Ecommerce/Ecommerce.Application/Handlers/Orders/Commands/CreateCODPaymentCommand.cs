using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace Ecommerce.Application.Handlers.Orders.Commands;

public class CreateCODPaymentCommand : IRequest<Response<string>>
{
    public AddCODPaymentDto AddCODPayment { get; set; }
}
public class CreateCODPaymentCommandHandler : IRequestHandler<CreateCODPaymentCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public CreateCODPaymentCommandHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Response<string>> Handle(CreateCODPaymentCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = _db.BeginTransaction();
        OrderPayment payment = new();
        var order = await _db.Orders.FindAsync(request.AddCODPayment.OrderId);
        if(order == null) return Response<string>.Fail("Sorry! No Order is Found to Add Payment.");
        payment.Reference = request.AddCODPayment.Reference;
        payment.PaymentType = PaymentMethod.CashOnDelivery;
        payment.Amount = request.AddCODPayment.Amount;
        payment.OrderId = request.AddCODPayment.OrderId;
        await _db.OrderPayments.AddAsync(payment, cancellationToken);

        order.PaymentStatus = PaymentStatus.Paid.ToString();
        _db.Orders.Update(order);

        var paymentReceivedStatus = await _db.OrderStatusValues.Where(o => o.StatusValue == DefaultOrderStatusValue.PaymentReceived().StatusValue).FirstOrDefaultAsync(cancellationToken);
        OrderStatus orderStatus2 = new OrderStatus()
        {
            OrderId = request.AddCODPayment.OrderId,
            OrderStatusValueId = paymentReceivedStatus.Id,
            Description = paymentReceivedStatus.Description
        };
        await _db.OrderStatus.AddAsync(orderStatus2, cancellationToken);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Response<string>.Fail("Failed to update");
        }
    }
}
