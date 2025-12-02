namespace PaymentDemoApp.Applicaiton.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateHostedTokenizationAsync();
    }
}