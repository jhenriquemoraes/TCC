using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentService.Interfaces;
using RabbitMQ.Client;

namespace PaymentService.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        public async Task PublishAsync<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: "payment_confirmed_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await channel.BasicPublishAsync(
                exchange: "payment_confirmed_exchange",
                routingKey: "",
                body: body);
        }
    }
}