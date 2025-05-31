using Ecommerce.Application.Interfaces;
using PayPal.Api;

namespace Ecommerce.Application.Services;

public class PaypalServices : IPaypalServices
{

    private readonly string _clientId;
    private readonly string _clientSecret;

    public PaypalServices()
    {
        _clientId = "Acte1Yk3odtdFcfNn1xI2tCsnkYSXascP82WT_UjCCMs-f7rxWRrl-iv_glQ-Y8_v3dyphiEOk09vYcr";
        _clientSecret = "EGIhAiEZMP6WScP-JHKx-LrnmscY0bQfJwm-TphCdmpum6ZhqV7BMWLxNrOIKFU_VMb0wjym0MXwdlvU";
    }

    public Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl, string intent, string invoiceNo)
    {
        var apiContext = new APIContext(new OAuthTokenCredential(_clientId, _clientSecret).GetAccessToken());

        var payment = new Payment()
        {
            intent = intent,
            payer = new Payer() { payment_method = "paypal" },
            transactions = GetTransactionsList(amount, invoiceNo),
            redirect_urls = new RedirectUrls()
            {
                cancel_url = cancelUrl,
                return_url = returnUrl
            }
        };

        var createdPayment = payment.Create(apiContext);

        return createdPayment;
    }


    private List<Transaction> GetTransactionsList(decimal amount, string invoiceNo)
    {
        var transactionList = new List<Transaction>();

        transactionList.Add(new Transaction()
        {
            description = "Transaction description.",
            invoice_number = invoiceNo,
            amount = new Amount()
            {
                currency = "USD",
                total = amount.ToString(),
                details = new Details()
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = amount.ToString()
                }
            },
            payee = new Payee
            {
                // TODO.. Enter the payee email address here
                email = "",
            }
        });

        return transactionList;
    }

    public Payment ExecutePayment(string paymentId, string payerId)
    {
        var apiContext = new APIContext(new OAuthTokenCredential(_clientId, _clientSecret).GetAccessToken());

        var paymentExecution = new PaymentExecution() { payer_id = payerId };

        var executedPayment = new Payment() { id = paymentId }.Execute(apiContext, paymentExecution);

        return executedPayment;
    }

    private string GetRandomInvoiceNumber()
    {
        return new Random().Next(999999999).ToString();
    }
}
