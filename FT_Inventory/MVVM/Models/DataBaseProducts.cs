using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.Core.Observable;

namespace FT_Inventory.MVVM.Models
{
    internal class DataBaseProducts : ObservableObject
    {
        private readonly DatabaseManager _dbManager;
        
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _productCategories;
        public ObservableCollection<string> ProductCategories
        {
            get => _productCategories;
            set
            {
                _productCategories = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        public DataBaseProducts(DatabaseManager dbManager)
        {
            _dbManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
            _products = new ObservableCollection<Product>(_dbManager.GetAllProducts());
            _customers = new ObservableCollection<Customer>(_dbManager.GetAllCustomers());
        }

        public bool UpdateProduct(Product product)
        {
            if (_dbManager.UpdateProduct(product))
            {
                OnPropertyChanged(nameof(Products));
                return true;
            }
            return false;
        }

        public bool DeleteProduct(Product product)
        {
            if (_dbManager.DeleteProduct(product) > 0)
            {
                OnPropertyChanged(nameof(Products));
                return true;
            }
            return false;
        }

        public bool UpdateCustomer(Customer customer)
        {
            if (_dbManager.UpdateCustomer(customer))
            {
                OnPropertyChanged(nameof(Customers));
                return true;
            }
            return false;
        }

        internal void DeleteProduct(int product)
        {
            throw new NotImplementedException();
        }
    }
}
