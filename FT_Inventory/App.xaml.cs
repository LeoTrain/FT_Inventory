using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Data;
using FT_Inventory.MVVM.Models;
using FT_Inventory.MVVM.ViewModel;

namespace FT_Inventory
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DatabaseManager? DbManager { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DbManager = new DatabaseManager("Server=DESKTOP-U0S8Q19\\SQLEXPRESS;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;");
            MainViewModel mainViewModel = new MainViewModel(DbManager);
            MainWindow mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();
        }
    }


}
