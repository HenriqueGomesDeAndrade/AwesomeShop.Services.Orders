using AwesomeShop.Services.Orders.Core.Repositories;
using AwesomeShop.Services.Orders.Infrastructure.MessageBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeShop.Services.Orders.Application.Commands.Handler
{
    public class AddOrderHandler : IRequestHandler<AddOrder, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IMessageBusClient _messageBusClient;
        public AddOrderHandler(IOrderRepository orderRepository, IMessageBusClient messageBusClient)
        {
            _orderRepository = orderRepository;
            _messageBusClient = messageBusClient;
        }

        public async Task<Guid> Handle(AddOrder request, CancellationToken cancellationToken)
        {
            var order = request.ToEntity();

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
