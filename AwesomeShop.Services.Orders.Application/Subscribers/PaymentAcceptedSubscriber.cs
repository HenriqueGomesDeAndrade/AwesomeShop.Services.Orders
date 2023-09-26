using AwesomeShop.Services.Orders.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeShop.Services.Orders.Application.Subscribers
{
    public class PaymentAcceptedSubscriber : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string Queue = "order-service/payment-accepted";
        private const string Exchange = "order-service";
        private const string RoutingKey = "payment-accepted";

        public PaymentAcceptedSubscriber(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            _connection = connectionFactory.CreateConnection("order-service-payment-accepted-subscriber");
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: Queue, exchange: "payment-service", routingKey: RoutingKey);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var contentString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                var message = JsonConvert.DeserializeObject<PaymentAccepted>(contentString);

                Console.WriteLine("Message received: {0}", message.Id);

                var result = await UpdateOrder(message);

                if(result)
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(Queue, false, consumer);
        }

        private async Task<bool> UpdateOrder(PaymentAccepted paymentAccepted)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var order = await orderRepository.GetByIdAsync(paymentAccepted.Id);
                order.SetAsCompleted();
                await orderRepository.UpdateAsync(order);
                return true;
            }
        }
    }

    public class PaymentAccepted
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
