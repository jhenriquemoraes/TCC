using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductService.Messaging
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IMessagePublisher _messagePublisher;
        public OrderCreatedConsumer(IMessagePublisher messagePublisher)
        {
            this._messagePublisher = messagePublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("ProductService Consumer iniciou.");
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "order_created_product_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = System.Text.Encoding.UTF8.GetString(body);

                var orderCreated =
                    System.Text.Json.JsonSerializer.Deserialize<OrderCreated>(message);

                if (orderCreated is null)
                {
                    Console.WriteLine("Could not deserialize OrderCreated.");
                    return;
                }

                Console.WriteLine($"Received message: {orderCreated.OrderId}");


                Console.WriteLine($"1 - OrderCreated recebido: {orderCreated.OrderId}");
                var productsReady = new ProductsReady
                {
                    OrderId = orderCreated.OrderId
                };
                Console.WriteLine("2 - Vou publicar ProductsReady");
                await _messagePublisher.PublishAsync(productsReady);

                Console.WriteLine($"3 - ProductsReady publicado {orderCreated.OrderId}");

                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
                Console.WriteLine("4 - ACK realizado");
            };

            await channel.BasicConsumeAsync(
                queue: "order_created_product_queue",
                autoAck: false,
                consumer: consumer);

            Console.WriteLine("Consumer registrado no RabbitMQ.");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}