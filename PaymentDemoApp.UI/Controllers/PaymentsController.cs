using Microsoft.AspNetCore.Mvc;
using PaymentDemoApp.Applicaiton.Interfaces;
using PaymentDemoApp.Applicaiton.Models;

namespace PaymentDemoApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentRequest request)
    {
        string clientSecret = await paymentService.CreatePaymentIntentAsync(
            request.Amount,
            request.Currency
        );
        
        return Ok(new { clientSecret });
    }
}