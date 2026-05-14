using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Mensagem recebida: {message}");

    await Task.CompletedTask;
};

await channel.BasicConsumeAsync(
    queue: "sales-queue",
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("Aguardando mensagens...");
Console.ReadLine();