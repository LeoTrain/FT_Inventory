using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    internal class OrderDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        private Customer _selectedCustomer;
        public Order CurrentOrder { get; set; }
        public bool IsNewOrder { get; }
        public List<Customer> AllCustomers { get; }
        public List<string> AllCustomersName { get; }
        public RelayCommand DeleteOrderItemCommand { get; }
        public string SelectedCustomer
        {
            get => _selectedCustomer.FullName;
            set
            {
                foreach (Customer customer in AllCustomers)
                {
                    if (customer.FullName == value)
                    {
                        _selectedCustomer = customer;
                        CurrentOrder.Customer = _selectedCustomer;
                        OnPropertyChanged(nameof(SelectedCustomer));
                        OnPropertyChanged(nameof(CurrentOrder));
                    }
                }
            }
        }
        public OrderDetailsViewModel(DatabaseManager dbManager, Order currentOrder, bool isNewOrder)
        {
            _dbManager = dbManager;
            CurrentOrder = currentOrder;
            IsNewOrder = isNewOrder;
            AllCustomers = this._dbManager.GetAllCustomers();
            AllCustomersName = AllCustomers.Select(c => c.FullName).ToList();
            _selectedCustomer = isNewOrder ? new Customer() : AllCustomers.Find(c => c.Id == currentOrder.Customer.Id);
            DeleteOrderItemCommand = new RelayCommand(o => this.DeleteOrderItem(o));
        }

        public void DeleteOrderItem(object orderItem)
        {
            var dialogresult = MessageBox.Show("Are you sure you want to delete this item?", "Delete Order Item", MessageBoxButton.YesNo);
            if (dialogresult == MessageBoxResult.No)
                return;
            OrderItem item = orderItem as OrderItem;
            CurrentOrder.RemoveOrderItem(item);
            CurrentOrder.CalculateTotalPrice();
            this._dbManager.DeleteOrderItem(item);
            OnPropertyChanged(nameof(CurrentOrder));
        }

        public void LoadCustomer()
        {
            CurrentOrder.Customer = _selectedCustomer;
        }
    }
}
