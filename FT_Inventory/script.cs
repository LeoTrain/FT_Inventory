using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory
{
    internal class script
    {

        public void Main()
        {
            Random rand = new Random();
            DatabaseManager dbManager = new DatabaseManager("Server=DESKTOP-U0S8Q19\\SQLEXPRESS;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;");
            for (int i = 0; i < 2; i++)
            {
                string stock_keeping_unit = $"Product bot {i} sku";
                string name = $"Product bot {i}";
                string description = $"Product bot {i} description";
                decimal price = rand.Next(1, 1000);
                int stockQuantity = rand.Next(1, 1000);
                string category = $"Product bot {i} category";
                string image_url = $"Product bot {i} image url";
                decimal discount = rand.Next(1, 100);
                bool isActive = rand.Next(0, 1) == 1;
                DateTime created_at = DateTime.Now;
                DateTime updated_at = DateTime.Now;
                Product product = new Product(i, stock_keeping_unit, name, description, price, stockQuantity, category, image_url, discount, isActive, created_at, updated_at);
                dbManager.InsertProduct(product);

            }
        }
    }
}
