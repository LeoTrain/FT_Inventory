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

        public Order()
        {
            OrderId = 0;
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
            TotalPrice = CalculateTotalPrice();
        }

        public decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (OrderItem item in OrderItems)
            {
                totalPrice += item.TotalPrice;
            }
            return totalPrice;
        }

        public void AddOrderItem(OrderItem item)
        {
            OrderItems.Add(item);
        }

        public void RemoveOrderItem(OrderItem item)
        {
            OrderItems.Remove(item);
        }
    }

}
