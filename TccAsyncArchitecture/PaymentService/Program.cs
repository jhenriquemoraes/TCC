using PaymentService.Interfaces;
using PaymentService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<OrderCreatedConsumer>();
builder.Services.AddTransient<IMessagePublisher, RabbitMqPublisher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();