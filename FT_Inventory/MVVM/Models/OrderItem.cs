using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }


        public OrderItem(object orderId)
        {
            OrderItemId = 0;
            Product = new Product();
            ProductId = Product.Id;
            if (orderId is int)
                OrderId = (int)orderId;
            else
                throw new ArgumentException("Order Id must be an integer");
            Quantity = 0;
            TotalPrice = 0;
        }
        public OrderItem(int orderItemId, Product product, int quantity, int orderId)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            Product = product;
            ProductId = product.Id;
            Quantity = quantity;
            TotalPrice = product.Price * quantity;
        }
    }
}
