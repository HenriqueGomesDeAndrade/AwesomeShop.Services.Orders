using AwesomeShop.Services.Orders.Application.Dtos.IntegrationDtos;
using AwesomeShop.Services.Orders.Core.Repositories;
using AwesomeShop.Services.Orders.Infrastructure.MessageBus;
using AwesomeShop.Services.Orders.Infrastructure.ServiceDiscovery;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeShop.Services.Orders.Application.Commands.Handler
{
    public class AddOrderHandler : IRequestHandler<AddOrder, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IMessageBusClient _messageBusClient;
        private readonly IServiceDiscoveryService _serviceDiscovery;
        public AddOrderHandler(IOrderRepository orderRepository, IMessageBusClient messageBusClient, IServiceDiscoveryService serviceDiscovery)
        {
            _orderRepository = orderRepository;
            _messageBusClient = messageBusClient;
            _serviceDiscovery = serviceDiscovery;
        }

        public async Task<Guid> Handle(AddOrder request, CancellationToken cancellationToken)
        {
            var order = request.ToEntity();

            var customerUrl = await _serviceDiscovery.GetServiceUri("CustomerServices", $"/api/customers/{order.Customer.Id}");

            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync(customerUrl);
            var stringResult = await result.Content.ReadAsStringAsync();
            var customerDto = JsonConvert.DeserializeObject<GetCustomerByIdDto>(stringResult);
            await _orderRepository.AddAsync(order);

            foreach (var @event in order.Events)
            {
                var routingKey = @event.GetType().Name.ToDashCase();
                _messageBusClient.Publish(@event, routingKey, "order-service");
            }

            return order.Id;
        }
    }
}
