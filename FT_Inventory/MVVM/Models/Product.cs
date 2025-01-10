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
        private int _id { get; set; }
        private string _stockKeepingUnit { get; set; }
        private string _name { get; set; }
        private string _description { get; set; }
        private decimal _price { get; set; }
        private int _stockQuantity { get; set; }
        private string _category { get; set; }
        private string _imageUrl { get; set; }
        private decimal _discount { get; set; }
        private bool _isActive { get; set; }
        private DateTime _createdAt { get; set; }
        private DateTime _updatedAt { get; set; }

        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Id must be a positive number");
                _id = value;
            }
        }

        public string StockKeepingUnit
        {
            get { return _stockKeepingUnit; }
            set
            {
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException("Stock Keeping Unit must be at least 1 character long");
                if (value.Length > 50)
                    throw new ArgumentOutOfRangeException("Stock Keeping Unit must be at most 50 characters long");
                _stockKeepingUnit = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException("Name must be at least 1 character long");
                if (value.Length > 255)
                    throw new ArgumentOutOfRangeException("Name must be at most 255 characters long");
                _name = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException("Description must be at least 1 character long");
                _description = value;
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Price must be a positive number");
                _price = value;
            }
        }

        public int StockQuantity
        {
            get { return _stockQuantity; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Stock Quantity must be a positive number");
                _stockQuantity = value;
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException("Category must be at least 1 character long");
                if (value.Length > 100)
                    throw new ArgumentOutOfRangeException("Category must be at most 100 characters long");
                _category = value;
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException("Image URL must be at least 1 character long");
                if (value.Length > 255)
                    throw new ArgumentOutOfRangeException("Image URL must be at most 255 characters long");
                _imageUrl = value;
            }
        }

        public decimal Discount
        {
            get { return _discount; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Discount must be a positive number");
                _discount = value;
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }

        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
            set
            {
                if (value < _createdAt)
                    throw new ArgumentOutOfRangeException("Updated at must be after created at");
                _updatedAt = value;
            }
        }

        public Product()
        {
            _id = 0;
            _stockKeepingUnit = "";
            _name = "";
            _description = "";
            _price = 0;
            _stockQuantity = 0;
            _category = "";
            _imageUrl = "";
            _discount = 0;
            _isActive = false;
            _createdAt = DateTime.Now;
            _updatedAt = DateTime.Now;
        }

        public Product(int id, string stockKeepingUnit, string name, string description, decimal price, int stockQuantity, string category, string imageUrl, decimal discount, bool isActive, DateTime createdAt, DateTime updatedAt)
        {
            _id = id;
            _stockKeepingUnit = stockKeepingUnit;
            _name = name;
            _description = description;
            _price = price;
            _stockQuantity = stockQuantity;
            _category = category;
            _imageUrl = imageUrl;
            _discount = discount;
            _isActive = isActive;
            _createdAt = createdAt;
            _updatedAt = updatedAt;
        }
    }
}
