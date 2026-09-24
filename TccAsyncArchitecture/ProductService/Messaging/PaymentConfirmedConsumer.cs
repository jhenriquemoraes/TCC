using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.Events;
using ProductService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductService.Messaging
{
    public class PaymentConfirmedConsumer : BackgroundService
    {
        private readonly IMessagePublisher _messagePublisher;
        public PaymentConfirmedConsumer(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
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

            await channel.ExchangeDeclareAsync(
                exchange: "payment_confirmed_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            await channel.QueueDeclareAsync(
                queue: "payment_confirmed_product_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: "payment_confirmed_product_queue",
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

                // ------ Saida no console e publicação 
                Console.WriteLine($"ProductService recebeu PaymentConfirmed: {paymentConfirmed.OrderId}");

                foreach (var product in paymentConfirmed.Products)
                {
                    Console.WriteLine(
                        $"Produto: {product.ProductId} | " +
                        $"Quantidade venda: {product.SaleQuantity} | " +
                        $"Valor venda: {product.SaleValue}");

                    var itemSeparated = new ProductItemSeparated
                    {
                        OrderId = paymentConfirmed.OrderId,
                        ProductId = product.ProductId,
                        SaleQuantity = product.SaleQuantity
                    };

                    await _messagePublisher.PublishAsync(itemSeparated);
                }
                
                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
                Console.WriteLine("4 - ACK realizado");
            };

            await channel.BasicConsumeAsync(
                queue: "payment_confirmed_product_queue",
                autoAck: false,
                consumer: consumer);

            Console.WriteLine("Consumer registrado no RabbitMQ.");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}