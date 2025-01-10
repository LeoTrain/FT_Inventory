using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsNewOrderItem { get; }
        public OrderItemViewModel(DatabaseManager _dbManager, OrderItem currentOrderItem, bool isNewOrderItem)
        {
            dbManager = _dbManager;
            _currentOrderItem = currentOrderItem;
            IsNewOrderItem = isNewOrderItem;
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

    }
}
