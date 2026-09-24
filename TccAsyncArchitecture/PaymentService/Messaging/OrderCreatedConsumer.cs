using MessageContracts.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PaymentService.Interfaces;

namespace PaymentService.Messaging
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IMessagePublisher _messagePublisher;
        public OrderCreatedConsumer(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
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

            await channel.ExchangeDeclareAsync(
                exchange: "order_created_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            await channel.QueueDeclareAsync(
                queue: "order_created_payment_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: "order_created_payment_queue",
                exchange: "order_created_exchange",
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

                var paymentApproved = !string.IsNullOrWhiteSpace(orderCreated.Payment.PaymentMethod);

                if (paymentApproved)
                {
                    var paymentConfirmed = new PaymentConfirmed
                    {
                        OrderId = orderCreated.OrderId,
                        Products = orderCreated.Products
                    };

                    await _messagePublisher.PublishAsync(paymentConfirmed);
                }
                else
                {
                    Console.WriteLine($"Payment not approved for OrderId: {orderCreated.OrderId}");
                }

                Console.WriteLine($"OrderCreated recebido: {orderCreated.OrderId}");


                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            };

            await channel.BasicConsumeAsync(
                queue: "order_created_payment_queue",
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}