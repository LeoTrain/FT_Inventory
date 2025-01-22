using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FT_Inventory.MVVM.ViewModel;
using FT_Inventory.MVVM.Models;
using System.Data.Common;
using FT_Inventory.Core.Exceptions;

namespace FT_Inventory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseManager dbManager;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                this.dbManager = new DatabaseManager("Server=B101-03;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;");
                DataContext = new MainViewModel(this.dbManager);
            }
            catch (DbConnectionException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to proceed?",
                          "Confirmation",
                          MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
                System.Windows.Application.Current.Shutdown();
        }

    }
}