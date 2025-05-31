using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Customer = Ecommerce.Domain.Entities.Customer;
using PaymentMethod = Ecommerce.Domain.Constants.PaymentMethod;
using Ecommerce.Domain.Identity.Constants;
using PayPal.Api;

namespace Ecommerce.Application.Handlers.Orders.Commands;

public class CreateOrderCheckoutCommand : IRequest<Response<string>>
{
    public CheckoutDto Checkout { get; set; }
}
public class CreateOrderCheckoutCommandHandler : IRequestHandler<CreateOrderCheckoutCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly ICookieService _cookie;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public CreateOrderCheckoutCommandHandler(IDataContext db, IMapper mapper, ICookieService cookie, ICurrentUser currentUser, IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _db = db;
        _mapper = mapper;
        _cookie = cookie;
        _currentUser = currentUser;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Response<string>> Handle(CreateOrderCheckoutCommand request, CancellationToken cancellationToken)
    {
        var status = false;
        long orderId = 0;
        var timeNow = DateTime.UtcNow;

        List<CartDto> cart = new();
        if (_cookie.Contains("shop-cart")) cart = JsonSerializer.Deserialize<List<CartDto>>(_cookie.Get("shop-cart"))!;
        if (cart == null || !cart.Any()) return Response<string>.Fail("Sorry! Your Cart is Empty!");

        var deliveryCharge = await _db.DeliveryMethods.Where(o => o.Id == request.Checkout.DeliveryMethod).Select(o => o.Price).FirstOrDefaultAsync(cancellationToken);

        await using var transaction = _db.BeginTransaction();

        try
        {
            #region Customer
            var customer = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(o => o.ApplicationUserId == _currentUser.UserId, cancellationToken) ?? new Customer();

            if (customer?.Id <= 0 || request?.Checkout?.WillSaveInfo == true)
            {
                customer.ShippingAddress = request?.Checkout?.ShippingAddress;
                if (customer?.Id <= 0)
                {
                    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);
                    if (user == null) return Response<string>.Fail("Sorry! No Registered User Found to Add Order!");

                    user.FirstName = request?.Checkout?.CustomerFirstName;
                    user.LastName = request?.Checkout?.CustomerLastName;
                    user.Email = request?.Checkout?.CustomerEmail;
                    user.PhoneNumber = request?.Checkout?.CustomerPhoneNumber;
                    user.LastModifiedDate = timeNow;
                    user.LastModifiedBy = _currentUser.UserName;
                    customer.User = user;
                    await _userManager.AddToRoleAsync(user, DefaultApplicationRoles.Customer); // add role to the user


                    await _signInManager.SignOutAsync(); // Sign the user out
                    await _signInManager.SignInAsync(user, isPersistent: false); // Sign the user back in
                }
                else
                {
                    customer.User.FirstName = request?.Checkout?.CustomerFirstName;
                    customer.User.LastName = request?.Checkout?.CustomerLastName;
                    customer.User.Email = request?.Checkout?.CustomerEmail;
                    customer.User.PhoneNumber = request?.Checkout?.CustomerPhoneNumber;
                    customer.User.LastModifiedDate = timeNow;
                    customer.User.LastModifiedBy = _currentUser.UserName;
                }

                _db.Customers.Update(customer);
            }

            #endregion

            await _db.SaveChangesAsync(cancellationToken);

            if(customer == null || customer?.Id <= 0) return Response<string>.Fail($"Sorry! No Customer Found to Add Order.");

            OrderPayment payment = new();
            var order = new Domain.Entities.Order
            {
                CustomerId = customer.Id,
                CustomerName = request.Checkout.CustomerFullName,
                ShippingAddress = request.Checkout.ShippingAddress,
                CustomerComment = request.Checkout.CustomerComment,
                CustomerEmail = request.Checkout.CustomerEmail,
                CustomerMobile = request.Checkout.CustomerPhoneNumber,
                DeliveryMethodId = request.Checkout.DeliveryMethod,
                PaymentMethod = request.Checkout.PaymentMethod,
                PaymentStatus = PaymentStatus.Unpaid.ToString(),
                DeliveryCharge = deliveryCharge,
                OrderAmount = cart.Select(o => o.Price * o.Qty).Sum()
            };

            var orderDetails = new List<OrderDetails>();
            var variantList = new List<Variant>();

            foreach (var item in cart)
            {
                var deductStock = await _db.Variants.FindAsync(item.VariantId);
                if (deductStock == null) return Response<string>.Fail($"Sorry! No Product Variant Found With Name [{item.Title}].");
                if (deductStock?.Qty < item.Qty) return Response<string>.Fail($"Sorry, [{item.Title}] Doesn't Have Enough Stock for Order. Please Reduce the Stock & Try Again.");
                deductStock.Qty -= item.Qty;
                variantList.Add(deductStock);
                //_db.Variants.Update(deductStock);

                var orderDetail = new OrderDetails
                {
                    ProductVariantId = item.VariantId,
                    ProductName = item.Title,
                    UnitPrice = item.Price,
                    Qty = item.Qty,
                };
                orderDetails.Add(orderDetail);
            }
            order.OrderDetails = orderDetails;

            _db.Variants.UpdateRange(variantList);
            await _db.Orders.AddAsync(order, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            orderId = order.Id;

            #region Payment (Paypal, Stripe)
            if (request.Checkout.PaymentMethod == PaymentMethod.Paypal)
            {
                if (request.Checkout?.PaypalModel?.TransactionId == null) return Response<string>.Fail("Please Provide Valid Payment Info!");

                payment.Reference = request.Checkout.PaypalModel.TransactionId;
                payment.PaymentType = request.Checkout.PaymentMethod;
                payment.Amount = (decimal)(order.OrderAmount + order.DeliveryCharge);
                payment.OrderId = orderId;
                await _db.OrderPayments.AddAsync(payment, cancellationToken);

                order.PaymentStatus = PaymentStatus.Paid.ToString();
                _db.Orders.Update(order);

                var paymentReceivedStatus = _db.OrderStatusValues.FirstOrDefault(o => o.StatusValue == DefaultOrderStatusValue.PaymentReceived().StatusValue);
                OrderStatus oStatus = new OrderStatus()
                {
                    OrderId = orderId,
                    OrderStatusValueId = paymentReceivedStatus?.Id,
                    Description = paymentReceivedStatus?.Description
                };
                await _db.OrderStatus.AddAsync(oStatus, cancellationToken);
            }


            if (request.Checkout.PaymentMethod == PaymentMethod.CardPayment)
            {
                string secretKey = _configuration.GetValue<string>("Stripe:SecretKey")!;
                string currency = _configuration.GetValue<string>("Stripe:Currency")!;

                if (request.Checkout?.StripeModel?.Token == null) return Response<string>.Fail("Please Provide Valid Card Info!");

                Stripe.StripeConfiguration.ApiKey = secretKey;
                var options = new Stripe.ChargeCreateOptions
                {
                    Amount = Decimal.ToInt32(order.OrderAmount + deliveryCharge) * 100,
                    Currency = currency,
                    Description = "Order ID: " + order.InvoiceNo,
                    Source = request.Checkout.StripeModel.Token,
                };
                var service = new Stripe.ChargeService();
                var charge = await service.CreateAsync(options, null, cancellationToken);
                var res = charge.Id;


                if (charge.Status.ToLower() == "succeeded")
                {
                    payment.Reference = charge.Id;
                    payment.PaymentType = request.Checkout.PaymentMethod;
                    payment.Amount = (decimal)(order.OrderAmount + order.DeliveryCharge);
                    payment.OrderId = orderId;
                    await _db.OrderPayments.AddAsync(payment, cancellationToken);

                    order.PaymentStatus = PaymentStatus.Paid.ToString();
                    _db.Orders.Update(order);

                    var paymentReceivedStatus = _db.OrderStatusValues.FirstOrDefault(o => o.StatusValue == DefaultOrderStatusValue.PaymentReceived().StatusValue);
                    var oStatus = new OrderStatus()
                    {
                        OrderId = orderId,
                        OrderStatusValueId = paymentReceivedStatus?.Id,
                        Description = paymentReceivedStatus?.Description
                    };
                    await _db.OrderStatus.AddAsync(oStatus, cancellationToken);
                }
            } 
            #endregion

            var pendingStatus = _db.OrderStatusValues.FirstOrDefault(o => o.StatusValue == DefaultOrderStatusValue.Pending().StatusValue);
            var orderStatus = new OrderStatus()
            {
                OrderId = orderId,
                OrderStatusValueId = pendingStatus?.Id,
                Description = pendingStatus?.Description
            };
            await _db.OrderStatus.AddAsync(orderStatus, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            status = true;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            status = false;
        }

        if (status)
        {
            _cookie.Remove("shop-cart");
            return Response<string>.Success(orderId.ToString());
        }
        return Response<string>.Fail("Failed to Add Item!");
    }
}
