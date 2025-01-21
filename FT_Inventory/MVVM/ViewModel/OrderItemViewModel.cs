using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    internal class OrderItemViewModel : ViewModelBase
    {
        private DatabaseManager dbManager;
        private OrderItem _currentOrderItem;
        private Product _selectedProduct;
        public RelayCommand IncrementQuantityCommand { get; set; }
        public RelayCommand DecrementQuantityCommand { get; set; }
        public OrderItem CurrentOrderItem
        {
            get { return _currentOrderItem; }
            set
            {
                _currentOrderItem = value;
                OnPropertyChanged(nameof(CurrentOrderItem));
            }
        }

        public Product CurrentProduct { get; private set; }
        public string SelectedProduct
        {
            get { return _selectedProduct.Name; }
            set
            {
                foreach (Product product in AllProducts)
                    if (product.Name == value)
                    {
                        _selectedProduct = product;
                        CurrentProduct = product;
                        OnPropertyChanged(nameof(SelectedProduct));
                        OnPropertyChanged(nameof(CurrentProduct));
                        this.LoadProduct();
                    }
            }
        }

        public List<Product> AllProducts { get; }
        public List<string> AllProductsName { get; }
        public bool IsNewOrderItem { get; }
        public OrderItemViewModel(DatabaseManager _dbManager, OrderItem currentOrderItem, bool isNewOrderItem)
        {
            dbManager = _dbManager;
            CurrentOrderItem = currentOrderItem;
            IsNewOrderItem = isNewOrderItem;
            AllProducts = this.dbManager.GetAllProducts();
            AllProductsName = AllProducts.Select(p => p.Name).ToList();
            _selectedProduct = AllProducts[0];
            IncrementQuantityCommand = new RelayCommand(this.IncrementQuantity);
            DecrementQuantityCommand = new RelayCommand(this.DecrementQuantity);

        }

        private void IncrementQuantity(object obj)
        {
            CurrentOrderItem.Quantity++;
            CurrentOrderItem.TotalPrice = CurrentOrderItem.Product.Price * CurrentOrderItem.Quantity;
            OnPropertyChanged(nameof(CurrentOrderItem));
        }

        private void DecrementQuantity(object obj)
        {
            if (CurrentOrderItem.Quantity > 0)
            {
                CurrentOrderItem.Quantity--;
                CurrentOrderItem.TotalPrice = CurrentOrderItem.Product.Price * CurrentOrderItem.Quantity;
                OnPropertyChanged(nameof(CurrentOrderItem));
            }
        }

        public void LoadProduct()
        {
            CurrentOrderItem.Product = _selectedProduct;
            OnPropertyChanged(nameof(CurrentOrderItem));
        }

    }
}
