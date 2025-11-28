using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace PaymentDemoApp.Pages;

public class CustomPaymentModel(IConfiguration config) : PageModel
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