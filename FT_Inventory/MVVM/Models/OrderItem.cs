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
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }


        public OrderItem()
        {
            OrderItemId = 0;
            Product = new Product();
            ProductId = Product.Id;
            Quantity = 0;
            TotalPrice = 0;

        }
        public OrderItem(int orderItemId, Product product, int quantity)
        {
            OrderItemId = orderItemId;
            Product = product;
            ProductId = product.Id;
            Quantity = quantity;
            TotalPrice = product.Price * quantity;
        }
    }
}
