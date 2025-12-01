using Microsoft.Extensions.Configuration;
using OnlinePayments.Sdk;
using OnlinePayments.Sdk.Domain;
using OnlinePayments.Sdk.Merchant;
using PaymentDemoApp.Applicaiton.Interfaces;
using PaymentDemoApp.Applicaiton.Models;
using PaymentResponse = PaymentDemoApp.Applicaiton.Models.PaymentResponse;

namespace PaymentDemoApp.Infrastructure.Services;

public class WorldlinePaymentService : IPaymentService, ITokenService
{
    private readonly IMerchantClient merchantClient;
    private readonly IClient client;
    private readonly string merchantId;

    public WorldlinePaymentService(IConfiguration config)
    {
        string apiKey = config["Worldline:ApiKey"];
        string apiSecret = config["Worldline:ApiSecret"];
        merchantId = config["Worldline:MerchantId"];
        Uri apiEndpoint = new Uri(config["Worldline:ApiUrl"]);

        client = Factory.CreateClient(new CommunicatorConfiguration
        {
            ApiKeyId = apiKey,
            SecretApiKey = apiSecret,
            ApiEndpoint = apiEndpoint,
            Integrator = merchantId
        });

        merchantClient = client.WithNewMerchant(merchantId);
    }
    
    public async Task<string> CreateHostedTokenizationAsync()
    {
        CreateHostedTokenizationRequest request = new CreateHostedTokenizationRequest
        {
            Variant = "Default.html"
        };

        var response = await client
            .WithNewMerchant(merchantId)
            .HostedTokenization
            .CreateHostedTokenization(request);

        return response.HostedTokenizationUrl;
    }

    public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return new PaymentResponse { Success = false, ErrorMessage = "Missing token" };
        }

        CreatePaymentRequest paymentRequest = new CreatePaymentRequest
        {
            HostedTokenizationId = request.Token,
            CardPaymentMethodSpecificInput = new CardPaymentMethodSpecificInput
            {
                ThreeDSecure = new ThreeDSecure
                {
                    SkipAuthentication = false,
                    RedirectionData = new RedirectionData
                    {
                        ReturnUrl = "http://localhost:5000/checkout-success"
                    }
                }
            },
            Order = new Order
            {
                AmountOfMoney = new AmountOfMoney
                {
                    Amount = request.Amount,
                    CurrencyCode = request.Currency.ToUpper()
                },
                Customer = new Customer
                {
                    Device = new CustomerDevice
                    {
                        AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                        Locale = "en_EN",
                        TimezoneOffsetUtcMinutes = "-180",
                        UserAgent = "Server-Side"
                    }
                }
            }
        };
        
        try
        {
            var resp = await merchantClient.Payments.CreatePayment(paymentRequest);

            return new PaymentResponse
            {
                Success = resp?.Payment != null,
                PaymentId = resp?.Payment?.Id,
                ErrorMessage = resp?.Payment == null ? "No payment created." : null
            };
        }
        catch (ValidationException vex)
        {
            Console.WriteLine("ValidationException: " + vex.Message);
            return new PaymentResponse { Success = false, ErrorMessage = vex.Message };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            return new PaymentResponse { Success = false, ErrorMessage = ex.Message };
        }

    }
}
