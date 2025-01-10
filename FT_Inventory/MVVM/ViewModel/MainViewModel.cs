using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FT_Inventory.Core;
using FT_Inventory.MVVM.View;
using System.Windows.Controls.Ribbon.Primitives;
using FT_Inventory.MVVM.Models;
using FT_Inventory.Core.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Data.SqlClient;

namespace FT_Inventory.MVVM.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly Stack<object> _viewHistory = new Stack<object>();
        private DatabaseManager _dbManager;
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand SwitchToHomeView { get; }
        public ICommand SwitchToProductsView { get; }
        public ICommand SwitchToCustomerView { get; }
        public ICommand SwitchToOrderView { get; }
        public ICommand SwitchToOrderDetailsView { get; }
        public ICommand SwitchToProductDetailsView { get; }
        public ICommand SwitchToCustomerDetailsView { get; }
        public ICommand GoBackCommand { get; }
        public ICommand AddNewProductCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand AddNewCustomerCommand { get; }
        public ICommand SaveCustomerCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public RelayCommand SwitchToOrderItemView { get; }
        public RelayCommand AddNewOrderCommand { get; }
        public RelayCommand SaveOrderCommand { get; }
        public RelayCommand AddNewOrderItem { get; }
        public RelayCommand SaveOrderItemCommand { get; }

        public MainViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            // Set HomeView as default view
            CurrentView = new HomeViewModel();

            // Initialize commands
            SwitchToProductDetailsView = new RelayCommand(o => this.NavigateTo(new ProductDetailsViewModel(_dbManager, o as Product, false)));
            SwitchToCustomerDetailsView = new RelayCommand(o => this.NavigateTo(new CustomerDetailsViewModel(_dbManager, o as Customer, false)));
            SwitchToOrderDetailsView = new RelayCommand(o => this.NavigateTo(new OrderDetailsViewModel(_dbManager, o as Order, false)));
            SwitchToOrderItemView = new RelayCommand(o => this.NavigateTo(new OrderItemViewModel(_dbManager, o as OrderItem, false)));

            SwitchToHomeView = new RelayCommand(o => this.NavigateTo(new HomeViewModel()));
            SwitchToProductsView = new RelayCommand(o => this.NavigateTo(new ProductsViewModel(_dbManager)));
            SwitchToCustomerView = new RelayCommand(o => CurrentView = new CustomersViewModel(_dbManager));
            SwitchToOrderView = new RelayCommand(o => CurrentView = new OrdersViewModel(_dbManager));

            GoBackCommand = new RelayCommand(o => GoBack());
            AddNewProductCommand = new RelayCommand(o => this.NavigateTo(new ProductDetailsViewModel(_dbManager, new Product(), true)));
            AddNewCustomerCommand = new RelayCommand(o => this.NavigateTo(new CustomerDetailsViewModel(_dbManager, new Customer(), true)));
            AddNewOrderCommand = new RelayCommand(o => this.NavigateTo(new OrderDetailsViewModel(_dbManager, new Order(), true)));
            AddNewOrderItem = new RelayCommand(o => this.NavigateTo(new OrderItemViewModel(_dbManager, new OrderItem(), true)));
            SaveProductCommand = new RelayCommand(o =>
            {
                if (o is Product product)
                {
                    if (product.Id == 0)
                        SaveNewProduct(product);
                    else
                        SaveExistingProduct(product);
                }
            });
            SaveCustomerCommand = new RelayCommand(o =>
            {
                if (o is Customer customer)
                {
                    if (customer.Id == 0)
                        SaveNewCustomer(customer);
                    else
                        SaveExistingCustomer(customer);
                }
            });
            SaveOrderCommand = new RelayCommand(o =>
            {
                if (o is Order order)
                {
                    if (order.OrderId == 0)
                        SaveNewOrder(order);
                    else
                        SaveExistingOrder(order);
                }
            });
            SaveOrderItemCommand = new RelayCommand(o =>
            {
                if (o is OrderItem orderItem)
                {
                    if (orderItem.OrderItemId == 0)
                        SaveNewOrderItem(orderItem);
                    else
                        SaveExistingOrderItem(orderItem);
                }
            });
        }

        public void NavigateTo(object newView)
        {
            if (CurrentView != null)
                _viewHistory.Push(CurrentView);
            CurrentView = newView;
        }

        public void GoBack()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to go back?", "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                if (_viewHistory.Count > 0)
                    CurrentView = _viewHistory.Pop();
        }

        public void SaveExistingProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                MessageBox.Show("Product name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_dbManager.UpdateProduct(product))
            {
                MessageBox.Show("Product saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.GoBack();
            }
            else
                MessageBox.Show("Error saving the product", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SaveExistingCustomer(Customer customer)
        {
            if (_dbManager.UpdateCustomer(customer))
            {
                MessageBox.Show("Customer saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.GoBack();
            }
            else
                MessageBox.Show("Error saving the customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SaveNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                MessageBox.Show("Product name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this._dbManager.InsertProduct(product);
            MessageBox.Show("Product saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            GoBack();
        }

        public void SaveNewCustomer(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.FirstName))
            {
                MessageBox.Show("Customers first name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(customer.LastName))
            {
                MessageBox.Show("Customers last name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(customer.Email))
            {
                MessageBox.Show("Customers email cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                this._dbManager.InsertCustomer(customer);
                MessageBox.Show("Customer saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CustomersViewModel cvm = this._viewHistory.Peek() as CustomersViewModel;
                cvm.AddCustomer(customer);
                GoBack();

            }
            catch (SqlException exception)
            {
                if (exception.Number == 2627)
                    MessageBox.Show("Error: Email Address is already used", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void SaveNewOrder(Order order)
        {
            try
            {
                this._dbManager.InsertOrder(order);
                MessageBox.Show("Order saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GoBack();
            }
            catch (SqlException exception)
            {
                if (exception.Number == 547)
                    MessageBox.Show("Error: Customer does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveExistingOrder(Order order)
        {
            if (_dbManager.UpdateOrder(order))
            {
                MessageBox.Show("Order saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.GoBack();
            }
            else
                MessageBox.Show("Error saving the order", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SaveNewOrderItem(OrderItem orderItem)
        {
            this._dbManager.InsertOrderItem(orderItem);
            MessageBox.Show("Order Item saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            GoBack();
        }

        public void SaveExistingOrderItem(OrderItem orderItem)
        {
            if (_dbManager.UpdateOrderItem(orderItem))
            {
                MessageBox.Show("Order Item saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.GoBack();
            }
            else
                MessageBox.Show("Error saving the order item", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
}
