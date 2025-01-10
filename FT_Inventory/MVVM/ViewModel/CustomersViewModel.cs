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
    class CustomersViewModel : ViewModelBase
    {
        public RelayCommand DeleteCustomerCommand { get; set; }
        private ObservableCollection<Customer> _customers { get; set; }
        public ObservableCollection<Customer> Customers { get { return _customers; } }
        private DatabaseManager _dbManager { get; set; }

        private Customer _selectedCustomer { get; set; }
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        public CustomersViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            _customers = new ObservableCollection<Customer>(_dbManager.GetAllCustomers());
            DeleteCustomerCommand = new RelayCommand(o => {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to proceed?",
                                                          "Confirmation",
                                                          MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (o is Customer customer)
                    {
                        Customers.Remove(customer);
                        dbManager.DeleteCustomer(customer.Id);
                    }
                }
            });
        }

        public void OverwriteCustomers(ObservableCollection<Customer> customers)
        {
            _customers = customers;
        }

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }
    }
}
