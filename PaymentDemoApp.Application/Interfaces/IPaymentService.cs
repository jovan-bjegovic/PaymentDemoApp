using PaymentDemoApp.Applicaiton.Models;

namespace PaymentDemoApp.Applicaiton.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);
}
