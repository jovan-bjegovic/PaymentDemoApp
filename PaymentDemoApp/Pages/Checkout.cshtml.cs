using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;

namespace PaymentDemoApp.Pages;

public class CheckoutModel(IConfiguration config) : PageModel
{
    public required string CheckoutUrl { get; set; }

    public void OnGet()
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            Mode = "payment",
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Test Product"
                        },
                        UnitAmount = 1000 // $10.00
                    },
                    Quantity = 1
                }
            ],
            SuccessUrl = "http://localhost:5000/CheckoutSuccess",
            CancelUrl = "http://localhost:5000/CheckoutCancel"
        };

        var service = new SessionService();
        var session = service.Create(options);

        CheckoutUrl = session.Url!;
    }
}