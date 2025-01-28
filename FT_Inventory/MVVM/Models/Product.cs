using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class Product
    {
        /// <summary>
        /// The unique identifier of the product in the database.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The stock keeping unit of the product in the database.
        /// </summary>
        public string StockKeepingUnit { get; set; }
        /// <summary>
        /// The name of the product in the database.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The description of the product in the database.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The price of the product in the database.
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The stock quantity of the product in the database.
        /// </summary>
        public int StockQuantity { get; set; }
        /// <summary>
        /// The category of the product in the database.
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// The image URL of the product in the database.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// The discount of the product in the database.
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// The status of the product in the database.  
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// The date and time the product was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// The date and time the product was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Default constructor for an empty product.
        /// </summary>
        public Product()
        {
            Id = 0;
            StockKeepingUnit = "";
            Name = "";
            Description = "";
            Price = 0;
            StockQuantity = 0;
            Category = "";
            ImageUrl = "";
            Discount = 0;
            IsActive = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Constructor for a full product from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stockKeepingUnit"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="stockQuantity"></param>
        /// <param name="category"></param>
        /// <param name="imageUrl"></param>
        /// <param name="discount"></param>
        /// <param name="isActive"></param>
        /// <param name="createdAt"></param>
        /// <param name="updatedAt"></param>
        public Product(int id, string stockKeepingUnit, string name, string description, decimal price, int stockQuantity, string category, string imageUrl, decimal discount, bool isActive, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            StockKeepingUnit = stockKeepingUnit;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            Category = category;
            ImageUrl = imageUrl;
            Discount = discount;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
