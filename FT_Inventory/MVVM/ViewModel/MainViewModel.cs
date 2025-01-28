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

        public RelayCommand ExitButtonCommand { get; }
        public RelayCommand SwitchToHomeView { get; }
        public RelayCommand SwitchToProductsView { get; }
        public RelayCommand SwitchToCustomerView { get; }
        public RelayCommand SwitchToOrderView { get; }
        public RelayCommand SwitchToOrderDetailsView { get; }
        public RelayCommand SwitchToProductDetailsView { get; }
        public RelayCommand SwitchToCustomerDetailsView { get; }
        public RelayCommand GoBackCommand { get; }
        public RelayCommand AddNewProductCommand { get; }
        public RelayCommand SaveProductCommand { get; }
        public RelayCommand AddNewCustomerCommand { get; }
        public RelayCommand SaveCustomerCommand { get; }
        public RelayCommand SwitchToOrderItemView { get; }
        public RelayCommand AddNewOrderCommand { get; }
        public RelayCommand SaveOrderCommand { get; }
        public RelayCommand AddNewOrderItem { get; }
        public RelayCommand SaveOrderItemCommand { get; }
        public bool HomeViewSelected { get; set; }
        public bool ProductsViewSelected { get; set;  }
        public bool CustomersViewSelected { get; set; }
        public bool OrdersViewSelected { get; set; }

        public MainViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            CurrentView = new HomeViewModel();
            this.SwitchSelected();
            ExitButtonCommand = new RelayCommand(o => { MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the Application?", "Confirmation", MessageBoxButton.YesNo); if (result == MessageBoxResult.Yes) System.Windows.Application.Current.Shutdown(); });
            SwitchToProductDetailsView = new RelayCommand(o => this.NavigateTo(new ProductDetailsViewModel(this._dbManager, o as Product, false)));
            SwitchToCustomerDetailsView = new RelayCommand(o => this.NavigateTo(new CustomerDetailsViewModel(this._dbManager, o as Customer, false)));
            SwitchToOrderDetailsView = new RelayCommand(o => this.NavigateTo(new OrderDetailsViewModel(this._dbManager, o as Order, false)));
            SwitchToOrderItemView = new RelayCommand(o => this.NavigateTo(new OrderItemViewModel(this._dbManager, o as OrderItem, false)));
            SwitchToHomeView = new RelayCommand(o => { this.NavigateTo(new HomeViewModel()); this.SwitchSelected(); });
            SwitchToProductsView = new RelayCommand(o => { this.NavigateTo(new ProductsViewModel(this._dbManager)); this.SwitchSelected(); });
            SwitchToCustomerView = new RelayCommand(o => { this.NavigateTo(new CustomersViewModel(this._dbManager)); this.SwitchSelected(); });
            SwitchToOrderView = new RelayCommand(o => { this.NavigateTo(new OrdersViewModel(this._dbManager)); this.SwitchSelected(); });
            GoBackCommand = new RelayCommand(o => this.GoBack());
            AddNewProductCommand = new RelayCommand(o => this.NavigateTo(new ProductDetailsViewModel(this._dbManager, new Product(), true)));
            AddNewCustomerCommand = new RelayCommand(o => this.NavigateTo(new CustomerDetailsViewModel(this._dbManager, new Customer(), true)));
            AddNewOrderCommand = new RelayCommand(o => this.NavigateTo(new OrderDetailsViewModel(this._dbManager, new Order(this._dbManager.GetLastOrderId()+1), true)));
            AddNewOrderItem = new RelayCommand(o => this.NavigateTo(new OrderItemViewModel(this._dbManager, new OrderItem(o), true)));
            SaveProductCommand = new RelayCommand(o =>
            {
                if (o is Product product)
                {
                    if (product.Id == 0) this.SaveNewProduct(product);
                    else this.SaveExistingProduct(product);
                    try
                    {
                        var productsViewModel = this._viewHistory.Peek() as ProductsViewModel;
                        if (productsViewModel != null) productsViewModel.LoadProducts();
                    }
                    catch (Exception ex) { MessageBox.Show($"Error changing the view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    this.GoBack();
                }
                else MessageBox.Show("Error saving the product, please report the bug.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            });
            SaveCustomerCommand = new RelayCommand(o =>
            {
                if (o is Customer customer)
                {
                    if (customer.Id == 0) this.SaveNewCustomer(customer);
                    else this.SaveExistingCustomer(customer);
                    this.GoBack();
                }
                else MessageBox.Show("Error saving the customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
            SaveOrderCommand = new RelayCommand(o =>
            {
                if (o is Order order)
                {
                    if (this._dbManager.OrderExists(order.OrderId)) this.SaveExistingOrder(order);
                    else this.SaveNewOrder(order);
                    this.GoBack();
                    try 
                    { 
                        var ordersViewModel = this.CurrentView as OrdersViewModel;
                        if (ordersViewModel != null) { ordersViewModel.Orders = new ObservableCollection<Order>(this._dbManager.GetAllOrders()); OnPropertyChanged(nameof(ordersViewModel.Orders)); }
                        MessageBox.Show("Order saved successfully !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    } 
                    catch (Exception ex) { MessageBox.Show($"Error changing the view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else MessageBox.Show("Error saving the order", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
            SaveOrderItemCommand = new RelayCommand(o =>
            {
                if (o is OrderItem orderItem)
                {
                    try
                    {
                        var oivm = CurrentView as OrderItemViewModel;
                        if (oivm != null)
                        {
                            oivm.LoadProduct();
                            if (oivm.CurrentOrderItem.OrderItemId == 0) this.SaveNewOrderItem(orderItem);
                            else this.SaveExistingOrderItem(oivm.CurrentOrderItem);
                            OnPropertyChanged(nameof(oivm.CurrentProduct));
                        }
                    }
                    catch (SqlException ex) { MessageBox.Show($"Error saving the order item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    catch (Exception ex) { MessageBox.Show($"Error changing the view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else MessageBox.Show("Unhandeld Error, please report the bug.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public void NavigateTo(object newView)
        {
            try
            {
                if (this.CurrentView != null) this._viewHistory.Push(CurrentView);
                this.CurrentView = newView;
                this.SwitchSelected();
            }
            catch (Exception ex) { MessageBox.Show($"Unhandled Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void GoBack()
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to go back?", "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) if (this._viewHistory.Count > 0) this.CurrentView = this._viewHistory.Pop();
            }
            catch (Exception ex) { MessageBox.Show($"Unhandled Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveExistingProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name)) { MessageBox.Show("Product name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (!float.TryParse(product.Price.ToString(), out _)) { MessageBox.Show("Price must be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (!int.TryParse(product.StockQuantity.ToString(), out _)) { MessageBox.Show("Quantity must be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (product.StockQuantity < 0) { MessageBox.Show("Quantity cannot be less than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            try
            {
                if (this._dbManager.UpdateProduct(product)) MessageBox.Show("Product saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Error saving the product", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveExistingCustomer(Customer customer)
        {
            try
            {
                if (_dbManager.UpdateCustomer(customer)) MessageBox.Show("Customer saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Error saving the customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException exception) { if (exception.Number == 2627) MessageBox.Show("Error: Email Address is already used", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name)) { MessageBox.Show("Product name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            try
            {
                this._dbManager.InsertProduct(product);
                MessageBox.Show("Product saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveNewCustomer(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.FirstName)) { MessageBox.Show("Customers first name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (string.IsNullOrEmpty(customer.LastName)) { MessageBox.Show("Customers last name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (string.IsNullOrEmpty(customer.Email)) { MessageBox.Show("Customers email cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            try
            {
                this._dbManager.InsertCustomer(customer);
                MessageBox.Show("Customer saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CustomersViewModel cvm = this._viewHistory.Peek() as CustomersViewModel;
                cvm.AddCustomer(customer);
                this.GoBack();

            }
            catch (SqlException exception) { if (exception.Number == 2627) MessageBox.Show("Error: Email Address is already used", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        public void SaveNewOrder(Order order)
        {
            try
            {
                OrderDetailsViewModel odvm = CurrentView as OrderDetailsViewModel;
                odvm.LoadCustomer();
                odvm.CurrentOrder.CalculateTotalPrice();
                if (this._dbManager.InsertOrder(order))
                    foreach (OrderItem orderItem in order.OrderItems) this._dbManager.InsertOrderItem(orderItem);
                else { MessageBox.Show("Error saving the order", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                MessageBox.Show("Order saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException exception) { if (exception.Number == 547) MessageBox.Show("Error: Customer does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveExistingOrder(Order order)
        {
            try
            {
                if (this._dbManager.UpdateOrder(order)) MessageBox.Show("Order saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Error saving the order", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveNewOrderItem(OrderItem orderItem)
        {
            try
            {
                OrderDetailsViewModel odvm = this._viewHistory.Peek() as OrderDetailsViewModel;
                orderItem.OrderItemId = this._dbManager.GetLasstOrderItemId() + 1;
                odvm.CurrentOrder.OrderItems.Add(orderItem);
                odvm.CurrentOrder.CalculateTotalPrice();
                this.OnPropertyChanged(nameof(odvm.CurrentOrder));
                MessageBox.Show("Order Item saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.GoBack();
            }
            catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public void SaveExistingOrderItem(OrderItem orderItem)
        {
            if (_dbManager.UpdateOrderItem(orderItem))
            {
                try
                {
                    OrderDetailsViewModel odvm = this._viewHistory.Peek() as OrderDetailsViewModel;
                    odvm.CurrentOrder.UpdateOrderItem(orderItem);
                    odvm.CurrentOrder.CalculateTotalPrice();
                    OnPropertyChanged(nameof(odvm.CurrentOrder));
                    MessageBox.Show("Order Item saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.GoBack();
                    OrdersViewModel ovm = this._viewHistory.Peek() as OrdersViewModel;
                    ovm.SelectedOrder = odvm.CurrentOrder;
                }
                catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else MessageBox.Show("Error saving the order item. Please save the Order if you created a new OrderItem", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SwitchSelected()
        {
            HomeViewSelected = CurrentView is HomeViewModel;
            ProductsViewSelected = CurrentView is ProductsViewModel;
            CustomersViewSelected = CurrentView is CustomersViewModel;
            OrdersViewSelected = CurrentView is OrdersViewModel;

            OnPropertyChanged(nameof(HomeViewSelected));
            OnPropertyChanged(nameof(ProductsViewSelected));
            OnPropertyChanged(nameof(CustomersViewSelected));
            OnPropertyChanged(nameof(OrdersViewSelected));
        }

    }
}
