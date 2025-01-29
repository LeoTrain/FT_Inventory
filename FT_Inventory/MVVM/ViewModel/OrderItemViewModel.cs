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
    /// <summary>
    /// The ViewModel for the OrderItemView. This ViewModel is used to display and edit an order item. Based on the <see cref="ViewModelBase"/>.
    /// </summary>
    internal class OrderItemViewModel : ViewModelBase
    {
        /// <summary>
        /// The database manager for the application.
        /// </summary>
        private DatabaseManager dbManager;
        /// <summary>
        /// The current order item being displayed.
        /// </summary>
        private OrderItem _currentOrderItem;
        /// <summary>
        /// The selected product for the order item.
        /// </summary>
        private Product _selectedProduct;
        /// <summary>
        /// The current product selected.
        /// </summary>
        private Product _currentProduct;
        /// <summary>
        /// The command to increment the quantity of the <see cref="SelectedProduct"/>.
        /// </summary>
        public RelayCommand IncrementQuantityCommand { get; set; }
        /// <summary>
        /// The command to decrement the quantity of the <see cref="SelectedProduct"/>.
        /// </summary>
        public RelayCommand DecrementQuantityCommand { get; set; }
        /// <summary>
        /// The current order item being displayed. After the order item is set, the <see cref="OnPropertyChanged"/> event is raised on it.
        /// </summary>
        public OrderItem CurrentOrderItem { get => _currentOrderItem; set { _currentOrderItem = value; OnPropertyChanged(nameof(CurrentOrderItem)); }}
        /// <summary>
        /// The selected product for the order item. After the product is set, the <see cref="OnPropertyChanged"/> event is raised on it.
        /// </summary>
        public Product CurrentProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(SelectedProduct); }}
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
                        if (IsNewOrderItem) { CurrentOrderItem.Quantity = 0; OnPropertyChanged(nameof(CurrentOrderItem)); }
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
            CurrentOrderItem.ProductId = currentOrderItem.Product.Id;
            IsNewOrderItem = isNewOrderItem;
            AllProducts = this.dbManager.GetAllProducts();
            AllProductsName = AllProducts.Select(p => p.Name).ToList();
            SelectedProduct = currentOrderItem.Product.Name;
            if (IsNewOrderItem)
            {
                CurrentOrderItem.Product = AllProducts[0];
                CurrentOrderItem.ProductId = AllProducts[0].Id;
                SelectedProduct = CurrentOrderItem.Product.Name;
            }
            IncrementQuantityCommand = new RelayCommand(this.IncrementQuantity);
            DecrementQuantityCommand = new RelayCommand(this.DecrementQuantity);

        }

        private void IncrementQuantity(object obj)
        {
            if (_selectedProduct.StockQuantity > CurrentOrderItem.Quantity + 1)
            {
                CurrentOrderItem.Quantity++;
                CurrentOrderItem.CalculateTotalPrice();
                OnPropertyChanged(nameof(CurrentOrderItem));
            }
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
            CurrentProduct = this.dbManager.GetProductById(_selectedProduct.Id);
            OnPropertyChanged(nameof(CurrentOrderItem));
            OnPropertyChanged(nameof(CurrentProduct));
        }

    }
}
