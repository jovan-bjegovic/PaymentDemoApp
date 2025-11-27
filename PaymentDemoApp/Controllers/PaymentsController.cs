using Microsoft.AspNetCore.Mvc;
using PaymentDemoApp.Models;
using Stripe;

namespace PaymentDemoApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    public PaymentsController(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    }

    [HttpPost("create-payment-intent")]
    public IActionResult CreatePaymentIntent([FromBody] PaymentRequest request)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount,
            Currency = "usd",
            PaymentMethodTypes = ["card"]
        };

        var service = new PaymentIntentService();
        var paymentIntent = service.Create(options);

        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }
}