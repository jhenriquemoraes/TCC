using InvoiceService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<PaymentConfirmedConsumer>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.Run();
