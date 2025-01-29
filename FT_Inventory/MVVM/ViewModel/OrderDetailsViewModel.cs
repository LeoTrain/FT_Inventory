using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    /// <summary>
    /// The ViewModel for the OrderDetailsView.
    /// </summary>
    internal class OrderDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        /// <summary>
        /// The selected customer for the order.
        /// </summary>
        private Customer _selectedCustomer;
        /// <summary>
        /// The current order being displayed.
        /// </summary>
        public Order CurrentOrder { get; set; }
        /// <summary>
        /// True if the order is a new order, false if the order is being edited.
        /// </summary>
        public bool IsNewOrder { get; }
        /// <summary>
        /// The <see cref="ObservableCollection{T}"/> of all customers in the database as <see cref="Customer"/>.
        /// </summary>
        public ObservableCollection<Customer> AllCustomers { get; }
        /// <summary>
        /// The <see cref="ObservableCollection{T}"/> of all customer names in the database as <see cref="string"/>.
        /// </summary>
        public ObservableCollection<string> AllCustomersName { get; }
        /// <summary>
        /// The command to delete an order item from the order.
        /// </summary>
        public RelayCommand DeleteOrderItemCommand { get; }
        /// <summary>
        /// The selected customer for the order. When the selected customer is changed, the <see cref="CurrentOrder"/> customer is updated.
        /// </summary>
        public string SelectedCustomer
        {
            get => _selectedCustomer.FullName;
            set
            {
                foreach (Customer customer in AllCustomers)
                    if (customer.FullName == value)
                    {
                        _selectedCustomer = customer;
                        CurrentOrder.Customer = _selectedCustomer;
                        OnPropertyChanged(nameof(SelectedCustomer));
                        OnPropertyChanged(nameof(CurrentOrder));
                    }
            }
        }
        /// <summary>
        /// The constructor for the <see cref="OrderDetailsViewModel"/>. The constructor initializes the <see cref="DatabaseManager"/>, the <see cref="CurrentOrder"/>, and the <see cref="IsNewOrder"/> properties.
        /// </summary>
        /// <param name="dbManager"></param>
        /// <param name="currentOrder"></param>
        /// <param name="isNewOrder"></param>
        public OrderDetailsViewModel(DatabaseManager dbManager, Order currentOrder, bool isNewOrder)
        {
            _dbManager = dbManager;
            CurrentOrder = currentOrder;
            IsNewOrder = isNewOrder;
            AllCustomers = new ObservableCollection<Customer>(this._dbManager.GetAllCustomers());
            AllCustomersName = new ObservableCollection<string>(AllCustomers.Select(c => c.FullName).ToList());
            List<Customer> customers = this._dbManager.GetAllCustomers();
            _selectedCustomer = isNewOrder ? AllCustomers[0] : currentOrder.Customer;
            SelectedCustomer = _selectedCustomer.FullName;
            DeleteOrderItemCommand = new RelayCommand(o => this.DeleteOrderItem(o));
        }

        /// <summary>
        /// Add an order item to the order. The order item is added to the <see cref="CurrentOrder"/> and the database. The total price of the order is recalculated. The <see cref="CurrentOrder"/> is updated.
        /// </summary>
        /// <param name="orderItem"></param>
        public void DeleteOrderItem(object orderItem)
        {
            var dialogresult = MessageBox.Show("Are you sure you want to delete this item?", "Delete Order Item", MessageBoxButton.YesNo);
            if (dialogresult == MessageBoxResult.No) return;
            OrderItem item = orderItem as OrderItem;
            this._dbManager.DeleteOrderItem(item);
            CurrentOrder = this._dbManager.GetOrderByOrderId(CurrentOrder.OrderId);
            OnPropertyChanged(nameof(CurrentOrder));
        }
    }
}
