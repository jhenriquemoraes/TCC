using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InvoiceService.Messaging
{
    public class PaymentConfirmedConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("InvoiceService Consumer iniciou.");
            ConnectionFactory factory = new ConnectionFactory()
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

            await channel.QueueDeclareAsync(
                queue: "payment_confirmed_invoice_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: "payment_confirmed_invoice_queue",
                exchange: "payment_confirmed_exchange",
                routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = System.Text.Encoding.UTF8.GetString(body);

                var paymentConfirmed = System.Text.Json.JsonSerializer.Deserialize<PaymentConfirmed>(message);

                if (paymentConfirmed is null)
                {
                    Console.WriteLine("Could not deserialize PaymentConfirmed.");
                    return;
                }

                Console.WriteLine($"InvoiceService recebeu PaymentConfirmed: {paymentConfirmed.OrderId}");

                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
                Console.WriteLine("4 - ACK realizado");
            };

            await channel.BasicConsumeAsync(
                queue: "payment_confirmed_invoice_queue",
                autoAck: false,
                consumer: consumer);

            Console.WriteLine("Consumer registrado no RabbitMQ.");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}