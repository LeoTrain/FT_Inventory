using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.Core;
using System.Collections.ObjectModel;
using FT_Inventory.MVVM.Models;
using FT_Inventory.Core.Commands;
using Microsoft.Win32;

namespace FT_Inventory.MVVM.ViewModel
{
    class CustomerDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        public Customer CurrentCustomer { get; set; }
        public bool IsNewCustomer { get; }
        public RelayCommand ChangeImageCommand { get; }
        public CustomerDetailsViewModel(DatabaseManager dbManager, Customer currentCustomer, bool isNewCustomer)
        {
            _dbManager = dbManager;
            CurrentCustomer = currentCustomer;
            ChangeImageCommand = new RelayCommand(o => this.ChangeImage());
            IsNewCustomer = isNewCustomer;
        }

        private void ChangeImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a file",
                Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CurrentCustomer.ImageUrl = openFileDialog.FileName;
                OnPropertyChanged(nameof(CurrentCustomer));
            }
        }
    }
}
