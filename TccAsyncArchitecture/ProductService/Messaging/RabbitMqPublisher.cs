using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProductService.Messaging
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
                exchange: "products_ready_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await channel.BasicPublishAsync(
                exchange: "products_ready_exchange",
                routingKey: "",
                body: body);
        }
    }
}