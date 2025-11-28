namespace PaymentDemoApp.Applicaiton.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(long amount, string currency);
}
