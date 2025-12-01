using PaymentDemoApp.Applicaiton.Interfaces;
using PaymentDemoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddScoped<IPaymentService, WorldlinePaymentService>();
builder.Services.AddScoped<ITokenService, WorldlinePaymentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

var config = builder.Configuration;
Stripe.StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

app.Run();