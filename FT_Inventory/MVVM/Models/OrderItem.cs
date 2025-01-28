using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class OrderItem
    {
        private int _quantity;
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        /// <summary>
        /// Quantity of the product in the order item. Cannot be negative. Total Price is calculated each time the quantity is set.
        /// </summary>
        public int Quantity { get => _quantity; set { if (value >= 0) { _quantity = value; this.CalculateTotalPrice(); }}}
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }


        /// <summary>
        /// Default constructor for an empty Order Item.
        /// </summary>
        /// <param name="orderId"></param>
        /// <exception cref="ArgumentException"></exception>
        public OrderItem(object orderId)
        {
            OrderItemId = 0;
            Product = new Product();
            ProductId = Product.Id;
            if (orderId is int) OrderId = (int)orderId; else throw new ArgumentException("Order Id must be an integer");
            Quantity = 0;
            TotalPrice = 0;
        }
        /// <summary>
        /// Constructor for the Order Item stored in the database.
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <param name="orderId"></param>
        public OrderItem(int orderItemId, Product product, int quantity, int orderId)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            Product = product;
            ProductId = product.Id;
            Quantity = quantity;
            TotalPrice = product.Price * quantity;
        }

        /// <summary>
        /// Calculate the total price of the order item.
        /// </summary>
        public void CalculateTotalPrice()
        {
            TotalPrice = Product.Price * Quantity;
        }
    }
}
