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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Warning | System.Diagnostics.SourceLevels.Error;
        }
    }

}
