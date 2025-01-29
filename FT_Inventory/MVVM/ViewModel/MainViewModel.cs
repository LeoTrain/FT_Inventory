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
        /// <summary>
        /// A stack to store the view history
        /// </summary>
        private readonly Stack<object> _viewHistory = new Stack<object>();
        /// <summary>
        /// The database manager to interact with the database
        /// </summary>
        private DatabaseManager _dbManager;
        /// <summary>
        /// The current view that is displayed in the main window
        /// </summary>
        private object _currentView;
        /// <summary>
        /// The current view that is displayed in the main window
        /// </summary>
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        /// <summary>
        /// Boolean propertie to check if the home view is selected
        /// </summary>
        public bool HomeViewSelected { get; set; }
        /// <summary>
        /// Boolean propertie to check if the products view is selected
        /// </summary>
        public bool ProductsViewSelected { get; set;  }
        /// <summary>
        /// Boolean propertie to check if the customers view is selected
        /// </summary>
        public bool CustomersViewSelected { get; set; }
        /// <summary>
        /// Boolean propertie to check if the orders view is selected
        /// </summary>
        public bool OrdersViewSelected { get; set; }

        /// <summary>
        /// Command to exit the application
        /// </summary>
        public RelayCommand ExitButtonCommand { get; }
        /// <summary>
        /// Command to switch to the home view
        /// </summary>
        public RelayCommand SwitchToHomeView { get; }
        /// <summary>
        /// Command to switch to the product details view
        /// </summary>
        public RelayCommand SwitchToProductsView { get; }
        /// <summary>
        /// Command to switch to the customer details view
        /// </summary>
        public RelayCommand SwitchToCustomerView { get; }
        /// <summary>
        /// Command to switch to the order details view
        /// </summary>
        public RelayCommand SwitchToOrderView { get; }
        /// <summary>
        /// Command to switch to the product details view
        /// </summary>
        public RelayCommand SwitchToOrderDetailsView { get; }
        /// <summary>
        /// Command to switch to the product details view
        /// </summary>
        public RelayCommand SwitchToProductDetailsView { get; }
        /// <summary>
        /// Command to switch to the customer details view
        /// </summary>
        public RelayCommand SwitchToCustomerDetailsView { get; }
        /// <summary>
        /// Command to switch to the order details view
        /// </summary>
        public RelayCommand GoBackCommand { get; }
        /// <summary>
        /// Command to switch to the product details view
        /// </summary>
        public RelayCommand AddNewProductCommand { get; }
        /// <summary>
        /// Command to save a already existing or new product
        /// </summary>
        /// <exception cref="Exception">If an error occurs</exception>
        public RelayCommand SaveProductCommand { get; }
        /// <summary>
        /// Command to switch to the customer details view
        /// </summary>
        public RelayCommand AddNewCustomerCommand { get; }
        /// <summary>
        /// Command to save an already existing or new customer
        /// </summary>
        public RelayCommand SaveCustomerCommand { get; }
        /// <summary>
        /// Command to switch to the order item view
        /// </summary>
        public RelayCommand SwitchToOrderItemView { get; }
        /// <summary>
        /// Command to switch to the order details view
        /// </summary>
        public RelayCommand AddNewOrderCommand { get; }
        /// <summary>
        /// Command to save an already existing or new order
        /// </summary>
        /// <exception cref="Exception">If an error occurs</exception>
        public RelayCommand SaveOrderCommand { get; }
        /// <summary>
        /// Command to add a new order item
        /// </summary>
        public RelayCommand AddNewOrderItem { get; }
        /// <summary>
        /// Command to save an already existing or new order item
        /// </summary>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>
        public RelayCommand SaveOrderItemCommand { get; }

        /// <summary>
        /// Constructor for the MainViewModel. Containing most of the Commands for the project. Uses a <see cref="DatabaseManager"/> to retrieve and save data from/to the database.
        /// </summary>
        /// <param name="dbManager"></param>
        public MainViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            CurrentView = new HomeViewModel();
            this.SwitchSelected();
            GoBackCommand = new RelayCommand(o => this.GoBack());
            ExitButtonCommand = new RelayCommand(o => { MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the Application?", "Confirmation", MessageBoxButton.YesNo); if (result == MessageBoxResult.Yes) System.Windows.Application.Current.Shutdown(); });
            SwitchToProductDetailsView = new RelayCommand(o => this.NavigateTo(new ProductDetailsViewModel(this._dbManager, o as Product, false)));
            SwitchToCustomerDetailsView = new RelayCommand(o => this.NavigateTo(new CustomerDetailsViewModel(this._dbManager, o as Customer, false)));
            SwitchToOrderDetailsView = new RelayCommand(o => this.NavigateTo(new OrderDetailsViewModel(this._dbManager, o as Order, false)));
            SwitchToOrderItemView = new RelayCommand(o => this.NavigateTo(new OrderItemViewModel(this._dbManager, o as OrderItem, false)));
            SwitchToHomeView = new RelayCommand(o => { this.NavigateTo(new HomeViewModel()); this.SwitchSelected(); });
            SwitchToProductsView = new RelayCommand(o => { this.NavigateTo(new ProductsViewModel(this._dbManager)); this.SwitchSelected(); });
            SwitchToCustomerView = new RelayCommand(o => { this.NavigateTo(new CustomersViewModel(this._dbManager)); this.SwitchSelected(); });
            SwitchToOrderView = new RelayCommand(o => { this.NavigateTo(new OrdersViewModel(this._dbManager)); this.SwitchSelected(); });
            AddNewProductCommand = new RelayCommand(o => this.NavigateTo(new ProductDetailsViewModel(this._dbManager, new Product(), true)));
            AddNewCustomerCommand = new RelayCommand(o => this.NavigateTo(new CustomerDetailsViewModel(this._dbManager, new Customer(), true)));
            AddNewOrderCommand = new RelayCommand(o => this.NavigateTo(new OrderDetailsViewModel(this._dbManager, new Order(this._dbManager.GetLastOrderId()+1), true)));
            AddNewOrderItem = new RelayCommand(o => this.NavigateTo(new OrderItemViewModel(this._dbManager, new OrderItem(o), true)));
            SaveProductCommand = new RelayCommand(o =>
            {
                if (o is Product product)
                {
                    bool save_result = false;
                    if (product.Id == 0) save_result = this.SaveNewProduct(product); else save_result = this.SaveExistingProduct(product);
                    try
                    {
                        var productsViewModel = this._viewHistory.Peek() as ProductsViewModel;
                        if (productsViewModel != null) productsViewModel.LoadProducts();
                    }
                    catch (Exception ex) { MessageBox.Show($"Error changing the view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    if (save_result) this.GoBack();
                }
                else MessageBox.Show("Error saving the product, please report the bug.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            });
            SaveCustomerCommand = new RelayCommand(o =>
            {
                bool save_result = false;
                if (o is Customer customer)
                {
                    if (customer.Id == 0) save_result = this.SaveNewCustomer(customer);
                    else save_result = this.SaveExistingCustomer(customer);
                    if (save_result) this.GoBack();
                }
                else MessageBox.Show("Error saving the customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
            SaveOrderCommand = new RelayCommand(o =>
            {
                if (o is Order order)
                {
                    bool order_saved = false;
                    if (this._dbManager.OrderExists(order.OrderId)) order_saved = this.SaveExistingOrder(order);
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
                            //OnPropertyChanged(nameof(oivm.CurrentProduct));
                        }
                    }
                    catch (SqlException ex) { MessageBox.Show($"Error saving the order item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    catch (Exception ex) { MessageBox.Show($"Error changing the view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else MessageBox.Show("Unhandeld Error, please report the bug.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        /// <summary>
        /// Method to navigate to a new view. It pushes the current view to a <see cref="Stack{T}"/> to be able to go back.
        /// </summary>
        /// <param name="newView"></param>
        /// <exception cref="Exception">If an error occurs</exception>
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

        /// <summary>
        /// Method to go back to the previous view. Careful it uses a <see cref="Stack{T}"/>, so it pops the current view.
        /// </summary>
        /// <exception cref="Exception">If an error occurs</exception>
        public void GoBack()
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to go back?", "Cancel Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes) if (this._viewHistory.Count > 0) this.CurrentView = this._viewHistory.Pop();
            }
            catch (Exception ex) { MessageBox.Show($"Unhandled Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        /// <summary>
        /// Method to save an existing product into the database.
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>
        public bool SaveExistingProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name)) { MessageBox.Show("Product name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (!float.TryParse(product.Price.ToString(), out _)) { MessageBox.Show("Price must be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (product.Price < 0) { MessageBox.Show("Price cannot be less than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (!int.TryParse(product.StockQuantity.ToString(), out _)) { MessageBox.Show("Quantity must be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (product.StockQuantity < 0) { MessageBox.Show("Quantity cannot be less than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            try
            {
                if (this._dbManager.UpdateProduct(product)) { MessageBox.Show("Product saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information); return true; }
                else { MessageBox.Show("Error saving the product", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            }
            catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            return false;
        }

        /// <summary>
        /// Method to save an existing customer into the database.
        /// </summary>
        /// <param name="customer"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>
        /// <returns>A <see cref="bool"/> that results if the customer has been saved.</returns>
        public bool SaveExistingCustomer(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.FirstName)) { MessageBox.Show("Customers first name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.FirstName.Count() > 50) { MessageBox.Show("Customers first name cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (string.IsNullOrEmpty(customer.LastName)) { MessageBox.Show("Customers last name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.LastName.Count() > 50) { MessageBox.Show("Customers last name cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (string.IsNullOrEmpty(customer.Email)) { MessageBox.Show("Customers email cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Email.Count() > 50) { MessageBox.Show("Customers email cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Phone.Count() > 25) { MessageBox.Show("Customers phone number cannot be longer than 25 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Address.Count() > 50) { MessageBox.Show("Customers address cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.City.Count() > 50) { MessageBox.Show("Customers city cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.State.Count() > 50) { MessageBox.Show("Customers state cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.PostalCode.Count() > 7) { MessageBox.Show("Customers postal code cannot be longer than 7 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Country.Count() > 50) { MessageBox.Show("Customers country cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            try
            {
                if (_dbManager.UpdateCustomer(customer)) { MessageBox.Show("Customer saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information); return true; }
                else { MessageBox.Show("Error saving the customer", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            }
            catch (SqlException exception) { if (exception.Number == 2627) MessageBox.Show("Error: Email Address is already used", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            return false;
        }

        /// <summary>
        /// Method to save a new product into the database.
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="Exception">If an error occurs</exception>
        public bool SaveNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.StockKeepingUnit)) { MessageBox.Show("StockKeepingUnit cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (string.IsNullOrEmpty(product.Name)) { MessageBox.Show("Product name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }

            try
            {
                this._dbManager.InsertProduct(product);
                MessageBox.Show("Product saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            return false;
        }

        /// <summary>
        /// Method to save a new customer into the database.
        /// </summary>
        /// <param name="customer"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        public bool SaveNewCustomer(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.FirstName)) { MessageBox.Show("Customers first name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.FirstName.Count() > 50) { MessageBox.Show("Customers first name cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (string.IsNullOrEmpty(customer.LastName)) { MessageBox.Show("Customers last name cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.LastName.Count() > 50) { MessageBox.Show("Customers last name cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (string.IsNullOrEmpty(customer.Email)) { MessageBox.Show("Customers email cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Email.Count() > 50) { MessageBox.Show("Customers email cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Phone.Count() > 25) { MessageBox.Show("Customers phone number cannot be longer than 25 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Address.Count() > 50) { MessageBox.Show("Customers address cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.City.Count() > 50) { MessageBox.Show("Customers city cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.State.Count() > 50) { MessageBox.Show("Customers state cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.PostalCode.Count() > 7) { MessageBox.Show("Customers postal code cannot be longer than 7 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (customer.Country.Count() > 50) { MessageBox.Show("Customers country cannot be longer than 50 characters", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            try
            {
                this._dbManager.InsertCustomer(customer);
                MessageBox.Show("Customer saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CustomersViewModel? cvm = this._viewHistory.Peek() as CustomersViewModel;
                cvm?.ReloadCustomers();
                return true;
            }
            catch (SqlException exception) { if (exception.Number == 2627) MessageBox.Show("Error: Email Address is already used", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            return false;
        }
        /// <summary>
        /// Method to save a new order into the database.
        /// </summary>
        /// <param name="order"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>
        public bool SaveNewOrder(Order order)
        {
            try
            {
                OrderDetailsViewModel? odvm = CurrentView as OrderDetailsViewModel;
                //odvm?.LoadCustomer();
                //odvm?.CurrentOrder.CalculateTotalPrice();
                if (odvm != null)
                {
                    bool result = this._dbManager.InsertOrder(order);
                    if (result) { foreach (OrderItem orderItem in order.OrderItems) this._dbManager.InsertOrderItem(orderItem); MessageBox.Show("Order saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information); }
                    else { MessageBox.Show("Error saving the order", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    return result;
                }
            }
            catch (SqlException exception) 
            { 
                if (exception.Number == 547) MessageBox.Show("Error: Customer does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
                else MessageBox.Show($"Unhandled SQL Error: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            return false;
        }

        /// <summary>
        /// Method to save an existing order into the database.
        /// </summary>
        /// <param name="order"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>
        public bool SaveExistingOrder(Order order)
        {
            bool result = false;
            try
            {
                result = this._dbManager.UpdateOrder(order); 
                if (result) MessageBox.Show("Order saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Error saving the order", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return result;
            }
            catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            return false;
        }

        /// <summary>
        /// Method to save a new order item into the database.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>
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

        /// <summary>
        /// Method to save an existing order item into the database.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <exception cref="SqlException">If an SQL error occurs</exception>
        /// <exception cref="Exception">If an error occurs</exception>  
        public void SaveExistingOrderItem(OrderItem orderItem)
        {
            if (_dbManager.UpdateOrderItem(orderItem))
            {
                try
                {
                    OrderDetailsViewModel? odvm = this._viewHistory.Peek() as OrderDetailsViewModel;
                    odvm?.CurrentOrder.UpdateOrderItem(orderItem);
                    odvm?.CurrentOrder.CalculateTotalPrice();
                    if (odvm != null) { 
                        OnPropertyChanged(nameof(odvm.CurrentOrder));
                        MessageBox.Show("Order Item saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.GoBack();
                        OrdersViewModel? ovm = this._viewHistory.Peek() as OrdersViewModel;
                        if (ovm != null) ovm.SelectedOrder = odvm.CurrentOrder;
                    }
                }
                catch (SqlException ex) { MessageBox.Show($"Unhandled SQL Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                catch (Exception ex) { MessageBox.Show($"Unhandled Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else MessageBox.Show("Error saving the order item. Please save the Order if you created a new OrderItem", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Method to switch the selected view and call the OnPropertyChanged method for the properties. (Is for the Menu bar, so that it stays selected)
        /// </summary>
        private void SwitchSelected()
        {
            HomeViewSelected = CurrentView is HomeViewModel;
            ProductsViewSelected = CurrentView is ProductsViewModel || CurrentView is ProductDetailsViewModel;
            CustomersViewSelected = CurrentView is CustomersViewModel || CurrentView is CustomerDetailsViewModel;
            OrdersViewSelected = CurrentView is OrdersViewModel || CurrentView is OrderDetailsViewModel || CurrentView is OrderItemViewModel;

            OnPropertyChanged(nameof(HomeViewSelected));
            OnPropertyChanged(nameof(ProductsViewSelected));
            OnPropertyChanged(nameof(CustomersViewSelected));
            OnPropertyChanged(nameof(OrdersViewSelected));
        }

    }
}
