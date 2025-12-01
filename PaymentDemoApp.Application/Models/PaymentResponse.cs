namespace PaymentDemoApp.Applicaiton.Models;

public class PaymentResponse
{
    public bool Success { get; set; }
    public string? PaymentId { get; set; } 
    public string? ErrorMessage { get; set; }
    public string? ClientSecret { get; set; }
}
