using Ecommerce.Application.Dto;
using Stripe;

namespace Ecommerce.Application.Helpers;

public class StripePayment
{
    public static async Task<dynamic> PayAsync(StripePaymentDto payModel)
    {
        try
        {
            StripeConfiguration.ApiKey = "sk_test_51K4UzeHVuxYHmXNNnOckn7oFStJId68UB26T5GKLYHAJJ6XE6Em5qUXmg4Sb7plWJPYu1SB9gVzexPpsyIjD8D5000xlnKkUTL";

            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = payModel.CardNumder,
                    ExpMonth = payModel.Month.ToString(),
                    ExpYear = payModel.Year.ToString(),
                    Cvc = payModel.CVC
                },
            };

            var serviceToken = new TokenService();
            Token stripeToken = await serviceToken.CreateAsync(options);

            var chargeOptions = new ChargeCreateOptions
            {
                Amount = payModel.Amount,
                Currency = "usd",
                Description = "Stripe Test Payment",
                Source = stripeToken.Id
            };

            var chargeService = new ChargeService();
            Charge charge = await chargeService.CreateAsync(chargeOptions);

            if (charge.Paid)
            {
                return "Success";
            }
            else
            {
                return "Failed";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
