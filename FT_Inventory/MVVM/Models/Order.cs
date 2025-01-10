using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public decimal TotalPrice { get; set; }

        public Order(Customer customer)
        {
            Customer = customer;
            OrderItems = new List<OrderItem>();
            CreatedAt = DateTime.Now;
            TotalPrice = 0;
        }

        public Order(int orderId)
        {
            OrderId = orderId;
            Customer = new Customer();
            CreatedAt = DateTime.Now;
            OrderItems = new List<OrderItem>();
            TotalPrice = 0;
        }
        public Order(int orderId, Customer customer, DateTime createdAt, List<OrderItem> orderItems)
        {
            OrderId = orderId;
            Customer = customer;
            CreatedAt = createdAt;
            OrderItems = orderItems;
            CalculateTotalPrice();
        }

        public void CalculateTotalPrice()
        {
            TotalPrice = 0;
            foreach (OrderItem item in OrderItems)
            {
                item.CalculateTotalPrice();
                TotalPrice += item.TotalPrice;
            }
        }

        public void AddOrderItem(OrderItem item)
        {
            OrderItems.Add(item);
        }

        public void RemoveOrderItem(OrderItem item)
        {
            OrderItems.Remove(item);
        }

        public void UpdateOrderItem(OrderItem item)
        {
            OrderItem orderItem = OrderItems.Find(i => i.OrderItemId == item.OrderItemId);
            if (orderItem != null)
            {
                orderItem.Product = item.Product;
                orderItem.Quantity = item.Quantity;
                orderItem.CalculateTotalPrice();
            }
        }
    }

}
