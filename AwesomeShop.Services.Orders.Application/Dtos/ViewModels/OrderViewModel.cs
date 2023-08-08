using AwesomeShop.Services.Orders.Core.Entitites;
using AwesomeShop.Services.Orders.Core.Enums;
using AwesomeShop.Services.Orders.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeShop.Services.Orders.Application.Dtos.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel(Guid id, decimal totalPrice, DateTime createdAt, string status)
        {
            Id = id;
            TotalPrice = totalPrice;
            CreatedAt = createdAt;
            Status = status;
        }

        public Guid Id { get; private set; }
        public decimal TotalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Status { get; private set; }

        public static OrderViewModel FromEntity(Order order)
        {
            return new OrderViewModel(order.Id, order.TotalPrice, order.CreatedAt, order.Status.ToString());
        }
    }
}
