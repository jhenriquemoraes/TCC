using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory
{
    HostName = "localhost"
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "sales-queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

string message = "Nova venda criada";

var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync(
    exchange: "",
    routingKey: "sales-queue",
    body: body
);

Console.WriteLine($"Mensagem enviada: {message}");