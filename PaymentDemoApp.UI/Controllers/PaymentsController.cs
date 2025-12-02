using Microsoft.AspNetCore.Mvc;
using PaymentDemoApp.Applicaiton.Interfaces;
using PaymentDemoApp.Applicaiton.Models;

namespace PaymentDemoApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService, ITokenService tokenService) : ControllerBase
{
    [HttpPost("create-token")]
    public async Task<IActionResult> CreateToken()
    {
        try
        {
            string url = await tokenService.CreateHostedTokenizationAsync();
            
            return Ok(new { token = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        try
        {
            PaymentResponse response = await paymentService.CreatePaymentAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new PaymentResponse { Success = false, ErrorMessage = ex.Message });
        }
    }

}