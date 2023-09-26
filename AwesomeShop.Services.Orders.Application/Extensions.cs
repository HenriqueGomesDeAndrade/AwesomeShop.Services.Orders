using AwesomeShop.Services.Orders.Application.Commands.Handler;
using AwesomeShop.Services.Orders.Application.Subscribers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeShop.Services.Orders.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddMediatR(typeof(AddOrderHandler).Assembly);

            return services;
        }

        public static IServiceCollection AddSubscribers(this IServiceCollection services)
        {
            services.AddHostedService<PaymentAcceptedSubscriber>();

            return services;
        }


        public static string ToDashCase(this string text)
        {
            return "";
        }
    }
}
