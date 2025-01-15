using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FT_Inventory.Core;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    class OrdersViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        private Order _selectedOrder { get; set; }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set 
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

        public RelayCommand DeleteOrderCommand { get; }

        public OrdersViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            Orders = new ObservableCollection<Order>(_dbManager.GetAllOrders());
            if (Orders.Count > 0)
                _selectedOrder = Orders[0];
            else
                _selectedOrder = new Order(0);
            DeleteOrderCommand = new RelayCommand(o => DeleteOrder());
        }

        public void DeleteOrder()
        {
            if (this.SelectedOrder != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this order?",
                                                          "Delete Confirmation",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    this._dbManager.DeleteOrder(SelectedOrder);
                    this.Orders.Remove(SelectedOrder);
                }
            }
        }
    }
}
