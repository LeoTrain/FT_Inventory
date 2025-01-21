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
        public ObservableCollection<Customer> Customers { get; set; }
        private DatabaseManager _dbManager { get; set; }
        private string _searchText;

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
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    this.UpdateCustomersBySearchText();
                }
            }
        }

        public CustomersViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            Customers = new ObservableCollection<Customer>(_dbManager.GetAllCustomers());
            if (Customers.Count > 0)
                _selectedCustomer = Customers[0];
            else
            {
                _selectedCustomer = new Customer();
                MessageBox.Show("No customers found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
           Customers = customers;
        }

        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer);
        }

        private void UpdateCustomersBySearchText()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                Customers = new ObservableCollection<Customer>(_dbManager.GetAllCustomers());
            else
            {
                var customersFromDb = _dbManager.GetAllCustomers();
                var filteredProducts = customersFromDb
                    .Where(p => p.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Customers = new ObservableCollection<Customer>(filteredProducts);
            }
            OnPropertyChanged(nameof(Customers));
        }
    }
}
