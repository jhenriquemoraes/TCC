using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DeliveryService.Messaging
{
    public class ProductItemSeparatedConsumer :  BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("DeliveryService Consumer iniciou.");
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: "product_item_separated_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            await channel.QueueDeclareAsync(
                queue: "product_item_separated_delivery_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: "product_item_separated_delivery_queue",
                exchange: "product_item_separated_exchange",
                routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = System.Text.Encoding.UTF8.GetString(body);

                var productItemSeparated = System.Text.Json.JsonSerializer.Deserialize<ProductItemSeparated>(message);

                if (productItemSeparated is null)
                {
                    Console.WriteLine("Could not deserialize ProductItemSeparated.");
                    return;
                }

                //------- Saida no Console 

                Console.WriteLine(
                    $"Item recebido para entrega - " +
                    $"Pedido: {productItemSeparated.OrderId} | " +
                    $"Produto: {productItemSeparated.ProductId} | " +
                    $"Quantidade: {productItemSeparated.SaleQuantity}");

                // -----------

                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            await channel.BasicConsumeAsync(
                queue: "product_item_separated_delivery_queue",
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }       

    }
}