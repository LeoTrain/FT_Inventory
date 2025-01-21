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
        public int Id { get; set; }
        public string StockKeepingUnit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public decimal Discount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

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
