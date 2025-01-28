using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.Core.Exceptions;
using Microsoft.Data.SqlClient;

namespace FT_Inventory.MVVM.Models
{
    public class DatabaseManager
    {
        private bool _isConnected;
        private string _connectionString;
        public bool IsConnected => _isConnected;
        public DatabaseManager(string connectionString)
        {
            try
            {
                _connectionString = connectionString;
                _isConnected = TryConnection();
            }
            catch (DbConnectionException)
            {
                _isConnected = false;
                throw;
            }

        }


        /// <summary>
        /// Tries to connect to the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DbConnectionException"></exception>
        private bool TryConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (SqlException e) when (e.Number == 208)
            {
                throw new DbConnectionException("Invalid object name. Please check your database schema.");
            }
            catch (SqlException e) when (e.Number == 53)
            {
                throw new DbConnectionException("The SQL Server instance could not be found. Please check the server name.");
            }
            catch (SqlException e) when (e.Number == 18456)
            {
                throw new DbConnectionException("Login failed for user. Please check your credentials.");
            }
            catch (SqlException e)
            {
                throw new DbConnectionException(e);
            }
        }

        /// <summary>
        /// Executes a query that returns a single value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="DbConnectionException"></exception>
        private T Execute<T>(Func<SqlCommand, T> action, string query, List<SqlParameter> parameters = null)
        {
            if (this._isConnected == false)
                throw new DbConnectionException();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters.ToArray());
                    connection.Open();
                    return action(command);
                }
            }
        }

        /// <summary>
        /// Executes a query that returns a single value.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <exception cref="DbConnectionException" />
        /// <returns></returns>
        private bool ExecuteNonQuery(string query, List<SqlParameter> parameters = null)
        {
            try
            {
                return this.Execute(command => command.ExecuteNonQuery(), query, parameters) > 0;
            }
            catch (DbConnectionException)
            {
                throw new DbConnectionException();
            }

        }

        /// <summary>
        /// Executes a query that returns a single value.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object ExecuteScalar(string query, List<SqlParameter> parameters = null)
        {
            try
            {
                return this.Execute(command => command.ExecuteScalar(), query, parameters);
            }
            catch (DbConnectionException)
            {
                throw new DbConnectionException();
            }
        }

        /// <summary>
        /// Executes a query that returns a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        private DataTable ExecuteQuery(string query, List<SqlParameter> parameters = null)
        {
            try
            {
                return Execute(command =>
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        if (adapter != null)
                            adapter.Fill(dataTable);
                        return dataTable;
                    }
                }, query, parameters);
            }
            catch (SqlException ex) when (ex.Number == 208) // SQL Server error code 208: Invalid object name
            {
                return new DataTable();
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during the query execution.", ex);
            }

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------- */

        /// <summary>
        /// This method inserts a product into the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool InsertProduct(Product product)
        {
            if (this._isConnected == false)
                return false;
            string query = "INSERT INTO Products (stock_keeping_unit, name, description, price, stock_quantity, category, image_url, discount, is_active) " +
                           "VALUES (@Sku, @Name, @Description, @Price, @StockQuantity, @Category, @ImageUrl, @Discount, @IsActive)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Sku", product.StockKeepingUnit),
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Description", product.Description ?? (object)DBNull.Value),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@StockQuantity", product.StockQuantity),
                new SqlParameter("@Category", product.Category ?? (object)DBNull.Value),
                new SqlParameter("@ImageUrl", product.ImageUrl ?? (object)DBNull.Value),
                new SqlParameter("@Discount", product.Discount),
                new SqlParameter("@IsActive", product.IsActive),
            };

            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method inserts a customer into the database.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool InsertCustomer(Customer customer)
        {
            if (this._isConnected == false)
                return false;
            string query = "INSERT INTO Customers (first_name, last_name, email, phone, address, city, state, postal_code, country, image_url) " +
                           "VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @City, @State, @PostalCode, @Country, @ImageUrl)";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
            new SqlParameter("@FirstName", customer.FirstName),
            new SqlParameter("@LastName", customer.LastName),
            new SqlParameter("@Email", customer.Email),
            new SqlParameter("@Phone", customer.Phone),
            new SqlParameter("@Address", customer.Address),
            new SqlParameter("@City", customer.City),
            new SqlParameter("@State", customer.State),
            new SqlParameter("@PostalCode", customer.PostalCode),
            new SqlParameter("@Country", customer.Country),
            new SqlParameter("@ImageUrl", customer.ImageUrl)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method inserts an order into the database.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool InsertOrder(Order order)
        {
            if (this._isConnected == false)
                return false;
            string query = "INSERT INTO orders (customer_id, created_at) VALUES (@CustomerId, GETDATE())";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", order.Customer.Id)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method inserts an order item into the database.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public bool InsertOrderItem(OrderItem orderItem)
        {
            string query = "INSERT INTO order_items (order_id, product_id, quantity, total_price) VALUES (@OrderId, @ProductId, @Quantity, @TotalPrice)";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderId", orderItem.OrderId),
                new SqlParameter("@ProductId", orderItem.Product.Id),
                new SqlParameter("@Quantity", orderItem.Quantity),
                new SqlParameter("@TotalPrice", orderItem.TotalPrice)
            };
            this.RemoveQuantityFromProduct(orderItem.Product.Id, orderItem.Quantity);
            return ExecuteNonQuery(query, parameters);
        }

        private bool RemoveQuantityFromProduct(int productId, int quantity)
        {
            string removeQuantityFromProductsQuery = "UPDATE products SET quantity = quantity - @Quantity WHERE product_id = @ProductId";
            List<SqlParameter> removeQuantityParameters = new List<SqlParameter>
            {
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@ProductId", productId)
            };
            return ExecuteNonQuery(removeQuantityFromProductsQuery, removeQuantityParameters);
        }
        /* -------------------------------------------------------------------------------------------------------------------------------------------------- */

        /// <summary>
        /// This method updates a product in the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool UpdateProduct(Product product)
        {
            string query = "UPDATE Products SET name = @Name, description = @Description, price = @Price, stock_quantity = @StockQuantity, " +
                           "category = @Category, image_url = @ImageUrl, discount = @Discount, is_active = @IsActive, updated_at = GETDATE() WHERE product_id = @ProductId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
            new SqlParameter("@ProductId", product.Id),
            new SqlParameter("@Name", product.Name),
            new SqlParameter("@Description", product.Description ?? (object)DBNull.Value),
            new SqlParameter("@Price", product.Price),
            new SqlParameter("@StockQuantity", product.StockQuantity),
            new SqlParameter("@Category", product.Category ?? (object)DBNull.Value),
            new SqlParameter("@ImageUrl", product.ImageUrl ?? (object)DBNull.Value),
            new SqlParameter("@Discount", product.Discount),
            new SqlParameter("@IsActive", product.IsActive),
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method updates a customer in the database.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdateCustomer(Customer customer)
        {
            string query = "UPDATE Customers SET first_name = @FirstName, last_name = @LastName, email = @Email, phone = @Phone, " +
                           "address = @Address, city = @City, state = @State, postal_code = @PostalCode, country = @Country, image_url = @ImageUrl, updated_at = GETDATE() WHERE customer_id = @CustomerId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
            new SqlParameter("@CustomerId", customer.Id),
            new SqlParameter("@FirstName", customer.FirstName),
            new SqlParameter("@LastName", customer.LastName),
            new SqlParameter("@Email", customer.Email),
            new SqlParameter("@Phone", customer.Phone),
            new SqlParameter("@Address", customer.Address),
            new SqlParameter("@City", customer.City),
            new SqlParameter("@State", customer.State),
            new SqlParameter("@PostalCode", customer.PostalCode),
            new SqlParameter("@Country", customer.Country),
            new SqlParameter("@ImageUrl", customer.ImageUrl)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method updates an order in the database.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool UpdateOrder(Order order)
        {
            foreach (OrderItem item in order.OrderItems)
            {
                if (this.OrderItemExists(item))
                    this.UpdateOrderItem(item);
                else
                    this.InsertOrderItem(item);
            }
            string query = "UPDATE orders SET customer_id = @CustomerId, created_at = @CreatedAt WHERE order_id = @OrderId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderId", order.OrderId),
                new SqlParameter("@CreatedAt", order.CreatedAt),
                new SqlParameter("@CustomerId", order.Customer.Id)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method updates an order item in the database.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public bool UpdateOrderItem(OrderItem orderItem)
        {
            string query = "UPDATE Order_Items SET order_id = @OrderId, product_id = @ProductId, quantity = @Quantity, total_price = @TotalPrice WHERE order_item_id = @OrderItemId";
            List<SqlParameter> parameter = new List<SqlParameter>
            {
                new SqlParameter("@OrderItemId", orderItem.OrderItemId),
                new SqlParameter("@OrderId", orderItem.OrderId),
                new SqlParameter("@ProductId", orderItem.ProductId),
                new SqlParameter("@Quantity", orderItem.Quantity),
                new SqlParameter("@TotalPrice", orderItem.TotalPrice)
            };
            return ExecuteNonQuery(query, parameter);
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------- */

        /// <summary>
        /// This method deletes a product from the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool DeleteProduct(Product product)
        {
            string query = "DELETE FROM Products WHERE product_id = @ProductId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProductId", product.Id)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method deletes a customer from the database.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public bool DeleteCustomer(int customerId)
        {
            string query = "DELETE FROM Customers WHERE customer_id = @CustomerId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", customerId)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method deletes an order from the database.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool DeleteOrder(Order order)
        {
            string query = "DELETE FROM orders WHERE order_id = @OrderId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderId", order.OrderId)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// This method deletes an order item from the database.
        /// </summary>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        public bool DeleteOrderItem(OrderItem orderItem)
        {
            string query = "DELETE FROM order_items WHERE order_item_id = @OrderItemId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderItemId", orderItem.OrderItemId)
            };
            return ExecuteNonQuery(query, parameters);
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------- */

        /// <summary>
        /// This method retrieves all products from the database.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetAllProducts()
        {
            string query = "SELECT product_id, stock_keeping_unit, name, description, price, stock_quantity, category, image_url, discount, is_active, created_at, updated_at FROM Products";
            try
            {
                DataTable dataTable = ExecuteQuery(query);
                List<Product> products = new List<Product>();
                foreach (DataRow row in dataTable.Rows)
                {
                    products.Add(new Product
                    (
                        (int)row["product_id"],
                        (string)row["stock_keeping_unit"],
                        (string)row["name"],
                        row["description"] as string,
                        (decimal)row["price"],
                        (int)row["stock_quantity"],
                        row["category"] as string,
                        row["image_url"] as string,
                        (decimal)row["discount"],
                        (bool)row["is_active"],
                        (DateTime)row["created_at"],
                        (DateTime)row["updated_at"]
                    ));
                }
                return products;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method retrieves all orders from the database.
        /// </summary>
        /// <returns></returns>
        public List<Order> GetAllOrders()
        {
            try
            {
                List<Product> products = this.GetAllProducts();
                List<Customer> customers = this.GetAllCustomers();
                string query = "SELECT order_id, customer_id, created_at FROM orders";
                DataTable dataTable = ExecuteQuery(query);
                List<Order> orders = new List<Order>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string orderItemQuery = "SELECT order_item_id, order_id, quantity FROM Order_Items WHERE order_id = @OrderId";
                    DataTable dataTable1 = ExecuteQuery(orderItemQuery, new List<SqlParameter> { new SqlParameter("@OrderId", row["order_id"]) });
                    List<OrderItem> orderItems = this.GetOrderItemsByOrderId((int)row["order_id"]);
                    orders.Add(new Order
                    (
                        (int)row["order_id"],
                        customers.FirstOrDefault(customer => customer.Id == (int)row["customer_id"]),
                        (DateTime)row["created_at"],
                        orderItems

                    ));
                }
                return orders;
            }
            catch (Exception)
            {
                return new List<Order>();
            }

        }

        /// <summary>
        /// This method retrieves all customers from the database.
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetAllCustomers()
        {
            string query = "SELECT customer_id, first_name, last_name, email, phone, address, city, state, postal_code, country, created_at, updated_at, image_url FROM Customers";
            DataTable dataTable = ExecuteQuery(query);
            List<Customer> customers = new List<Customer>();
            foreach (DataRow row in dataTable.Rows)
            {
                customers.Add(new Customer
                (
                    (int)row["customer_id"],
                    (string)row["first_name"],
                    (string)row["last_name"],
                    (string)row["email"],
                    (string)row["phone"],
                    (DateTime)row["created_at"],
                    (DateTime)row["updated_at"],
                    row["address"] as string,
                    row["city"] as string,
                    row["state"] as string,
                    row["postal_code"] as string,
                    row["country"] as string,
                    row["image_url"] as string
                ));
            }
            return customers;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------- */

        /// <summary>
        /// This method retrieves all products by its ID.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<Product> GetProductsByCategory(string category)
        {
            string query = "SELECT product_id, stock_keeping_unit, name, description, price, stock_quantity, category, image_url, discount, is_active, created_at, updated_at FROM Products WHERE category = @Category";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Category", category)
            };
            DataTable dataTable = ExecuteQuery(query, parameters);
            List<Product> products = new List<Product>();
            foreach (DataRow row in dataTable.Rows)
            {
                products.Add(new Product
                (
                    (int)row["product_id"],
                    (string)row["stock_keeping_unit"],
                    (string)row["name"],
                    row["description"] as string,
                    (decimal)row["price"],
                    (int)row["stock_quantity"],
                    row["category"] as string,
                    row["image_url"] as string,
                    (decimal)row["discount"],
                    (bool)row["is_active"],
                    (DateTime)row["created_at"],
                    (DateTime)row["updated_at"]
                ));
            }
            return products;
        }

        public string[] GetAllProductCategories()
        {
            string query = "SELECT DISTINCT category FROM Products";
            DataTable dataTable = ExecuteQuery(query);
            List<string> categories = new List<string>();
            categories.Add("All");
            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(row["category"] as string);
            }
            return categories.ToArray();
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<Product> products = this.GetAllProducts();
            string query = "SELECT order_item_id, order_id, product_id, quantity, total_price FROM order_items WHERE order_id = @OrderId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderId", orderId)
            };
            DataTable dataTable = ExecuteQuery(query, parameters);
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                orderItems.Add(new OrderItem
                (
                    (int)row["order_item_id"],
                    products.FirstOrDefault(product => product.Id == (int)row["product_id"]),
                    (int)row["quantity"],
                    (int)row["order_id"]
                ));
            }
            return orderItems;
        }

        public int GetLastOrderId()
        {
            try
            {
                string query = "SELECT MAX(order_id) FROM orders";
                return (int)ExecuteScalar(query);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetLasstOrderItemId()
        {
            try
            {
                string query = "SELECT MAX(order_item_id) FROM order_items";
                return (int)ExecuteScalar(query);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool OrderExists(int orderId)
        {
            string query = "SELECT COUNT(*) FROM orders WHERE order_id = @OrderID";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderId", orderId)
            };
            int count = (int)ExecuteScalar(query, parameters);
            return count > 0;
        }

        public bool OrderItemExists(OrderItem item)
        {
            string query = "SELECT COUNT(*) FROM order_items WHERE order_item_id = @OrderItemId";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderItemId", item.OrderItemId)
            };
            int count = (int)ExecuteScalar(query, parameters);
            return count > 0;
        }

    }
}
