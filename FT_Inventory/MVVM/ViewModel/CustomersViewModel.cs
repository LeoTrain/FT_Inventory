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
        /// <summary>
        /// Command to delete a customer.
        /// </summary>
        public RelayCommand DeleteCustomerCommand { get; set; }
        /// <summary>
        /// The list of customers. This list is updated when the search text is changed.
        /// </summary>
        public ObservableCollection<Customer> Customers { get; set; }
        /// <summary>
        /// The database manager to interact with the database.
        /// </summary>
        private DatabaseManager _dbManager { get; set; }
        /// <summary>
        /// The search text to filter the customers.
        /// </summary>
        private string _searchText;
        /// <summary>
        /// The selected customer in the list.
        /// </summary>
        private Customer _selectedCustomer { get; set; }
        /// <summary>
        /// The selected customer in the list. When the selected customer is changed, the UI is updated.
        /// </summary>
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }
        /// <summary>
        /// The search text to filter the customers. When the search text is changed, the customers list is updated.
        /// </summary>
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

        /// <summary>
        /// Constructor for the CustomersViewModel. This view model is used to display the list of customers.
        /// </summary>
        /// <param name="dbManager"></param>
        public CustomersViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            Customers = new ObservableCollection<Customer>(_dbManager.GetAllCustomers());
            if (Customers.Count > 0) SelectedCustomer = Customers[0];
            else { SelectedCustomer = new Customer(); MessageBox.Show("No customers found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            DeleteCustomerCommand = new RelayCommand(o => {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) if (o is Customer customer) { Customers.Remove(customer); dbManager.DeleteCustomer(customer.Id); }
            });
        }

        /// <summary>
        /// Add a customer to the list of customers. This method is used to add a new customer to the database.
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer);
        }

        /// <summary>
        /// Update the list of customers based on the search text. If the search text is empty, all customers are displayed.
        /// </summary>
        private void UpdateCustomersBySearchText()
        {
            if (string.IsNullOrWhiteSpace(SearchText)) Customers = new ObservableCollection<Customer>(_dbManager.GetAllCustomers());
            else
            {
                var customersFromDb = _dbManager.GetAllCustomers();
                var filteredProducts = customersFromDb.Where(p => p.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                Customers = new ObservableCollection<Customer>(filteredProducts);
                OnPropertyChanged(nameof(Customers));
            }
        }
    }
}
