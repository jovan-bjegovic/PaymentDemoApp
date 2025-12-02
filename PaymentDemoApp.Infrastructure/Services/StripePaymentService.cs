using Stripe;
using Microsoft.Extensions.Configuration;
using PaymentDemoApp.Applicaiton.Interfaces;
using PaymentDemoApp.Applicaiton.Models;

namespace PaymentDemoApp.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    public StripePaymentService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    }

    public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
    {
        var service = new PaymentIntentService();

        var createOptions = new PaymentIntentCreateOptions
        {
            Amount = request.Amount,             
            Currency = request.Currency.ToLower(),
            PaymentMethodTypes = ["card"],
            Confirm = false                           
        };

        try
        {
            var intent = await service.CreateAsync(createOptions);

            return new PaymentResponse
            {
                Success = true,
                PaymentId = intent.Id,
                ClientSecret = intent.ClientSecret,
                ErrorMessage = null
            };
        }
        catch (StripeException ex)
        {
            return new PaymentResponse
            {
                Success = false,
                PaymentId = null,
                ErrorMessage = ex.Message
            };
        }
    }
}