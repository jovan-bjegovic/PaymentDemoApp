namespace PaymentDemoApp.Applicaiton.Models;

public class PaymentRequest
{
    public required long Amount { get; set; }
    public required string Currency { get; set; }

    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Country { get; set; }
    public string? Token { get; set; }
}

