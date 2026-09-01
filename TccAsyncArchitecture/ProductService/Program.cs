using ProductService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
