using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace OrderApi.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        public async Task PublishAsync<T>(T message)
        {
            ConnectionFactory factory = new
                ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: "order_created_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            await channel.QueueDeclareAsync(
                queue: "order_created_product_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
                
            await channel.QueueBindAsync(
                queue: "order_created_product_queue",
                exchange: "order_created_exchange",
                routingKey: "");

            var jsonMessage = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await channel.BasicPublishAsync(
                exchange: "order_created_exchange",
                routingKey: "",
                body: body);
        }
    }
}   