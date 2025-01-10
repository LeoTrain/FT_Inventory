using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;
using FT_Inventory.MVVM.ViewModel;

namespace FT_Inventory.MVVM.View
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        private DatabaseManager _dbManager;
        public ProductsView()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager("Server=desktop-u0s8q19\\SQLEXPRESS;Database=InventoryDb;User Id=tom;Password=1234;TrustServerCertificate=True;");
            this.DataContext = new ProductsViewModel(_dbManager);
        }
    }
}
