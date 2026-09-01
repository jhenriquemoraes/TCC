using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentService.Messaging
{
    public class ProductsReadyConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "products_ready_payment_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: "products_ready_payment_queue",
                exchange: "products_ready_exchange",
                routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message =
                    System.Text.Encoding.UTF8.GetString(body);

                var orderCreated =
                    System.Text.Json.JsonSerializer
                        .Deserialize<OrderCreated>(message);

                if (orderCreated is null)
                {
                    Console.WriteLine("Could not deserialize OrderCreated.");
                    return;
                }

                Console.WriteLine($"PaymentService recebido: {orderCreated.OrderId}");
                Console.WriteLine($"Pagamento/nota fechada para o pedido {orderCreated.OrderId}");

                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            await channel.BasicConsumeAsync(
                queue: "products_ready_payment_queue",
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}