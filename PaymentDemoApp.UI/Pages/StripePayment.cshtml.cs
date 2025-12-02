using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PaymentDemoApp.Pages;

public class StripePaymentModel(IConfiguration config) : PageModel
{
    public required string PublishableKey { get; set; }

    public void OnGet()
    {
        string? key = config["Stripe:PublishableKey"];
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Stripe: PublishableKey is not defined in appsettings.json");
        }

        PublishableKey = key;
    }
}