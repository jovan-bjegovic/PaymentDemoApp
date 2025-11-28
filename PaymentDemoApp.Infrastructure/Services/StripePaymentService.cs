using Stripe;
using Microsoft.Extensions.Configuration;
using PaymentDemoApp.Applicaiton.Interfaces;

namespace PaymentDemoApp.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    public StripePaymentService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    }

    public async Task<string> CreatePaymentIntentAsync(long amount, string currency)
    {
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = currency.ToLower(),
            PaymentMethodTypes = ["card"]
        };
        
        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);
        
        return intent.ClientSecret;
    }
}
