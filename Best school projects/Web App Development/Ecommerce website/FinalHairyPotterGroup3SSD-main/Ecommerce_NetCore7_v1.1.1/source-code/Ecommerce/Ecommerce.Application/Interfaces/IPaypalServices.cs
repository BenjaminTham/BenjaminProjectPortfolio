using PayPal.Api;

namespace Ecommerce.Application.Interfaces;

public interface IPaypalServices
{
    Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl, string intent, string invoiceNo);
    Payment ExecutePayment(string paymentId, string payerId);
}
