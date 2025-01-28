using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class Order
    {
        /// <summary>
        /// The unique identifier of the order in the database.
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// The customer in the database who placed the order.
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// The date and time the order was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// The list of order items in the order.
        /// </summary>
        public List<OrderItem> OrderItems { get; set; }
        /// <summary>
        /// The total price of the order. The total price is the sum of the total price of all order items.
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Default constructor for an empty order.
        /// </summary>
        /// <param name="customer"></param>
        public Order(Customer customer)
        {
            Customer = customer;
            OrderItems = new List<OrderItem>();
            CreatedAt = DateTime.Now;
            TotalPrice = 0;
        }

        /// <summary>
        /// Constructor for an order with an order id.
        /// </summary>
        /// <param name="orderId"></param>
        public Order(int orderId)
        {
            OrderId = orderId;
            Customer = new Customer();
            CreatedAt = DateTime.Now;
            OrderItems = new List<OrderItem>();
            TotalPrice = 0;
        }
        /// <summary>
        /// Constructor for an order with an order id, customer, created at date, and order items.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="customer"></param>
        /// <param name="createdAt"></param>
        /// <param name="orderItems"></param>
        public Order(int orderId, Customer customer, DateTime createdAt, List<OrderItem> orderItems)
        {
            OrderId = orderId;
            Customer = customer;
            CreatedAt = createdAt;
            OrderItems = orderItems;
            CalculateTotalPrice();
        }

        /// <summary>
        /// Calculate the total price of the order. The total price is the sum of the total price of all order items.
        /// </summary>
        public void CalculateTotalPrice()
        {
            TotalPrice = 0;
            foreach (OrderItem item in OrderItems)
            {
                item.CalculateTotalPrice();
                TotalPrice += item.TotalPrice;
            }
        }

        /// <summary>
        /// Add an order item to the order.
        /// </summary>
        /// <param name="item"></param>
        public void AddOrderItem(OrderItem item)
        {
            OrderItems.Add(item);
        }

        /// <summary>
        /// Remove an order item from the order.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveOrderItem(OrderItem item)
        {
            OrderItems.Remove(item);
        }

        /// <summary>
        /// Update an order item in the order.
        /// </summary>
        /// <param name="item"></param>
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
