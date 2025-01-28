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
        /// <summary>
        /// The database manager to interact with the database.
        /// </summary>
        private DatabaseManager _dbManager;
        /// <summary>
        /// The current customer being added or edited.
        /// </summary>
        public Customer CurrentCustomer { get; set; }
        /// <summary>
        /// True if the customer is new, false if the customer is being edited.
        /// </summary>
        public bool IsNewCustomer { get; }
        /// <summary>
        /// Command to change the image of the customer.
        /// </summary>
        public RelayCommand ChangeImageCommand { get; }
        /// <summary>
        /// Constructor for the CustomerDetailsViewModel. This view model is used to add or edit a customer.
        /// </summary>
        /// <param name="dbManager"></param>
        /// <param name="currentCustomer"></param>
        /// <param name="isNewCustomer"></param>
        public CustomerDetailsViewModel(DatabaseManager dbManager, Customer currentCustomer, bool isNewCustomer)
        {
            _dbManager = dbManager;
            CurrentCustomer = currentCustomer;
            ChangeImageCommand = new RelayCommand(o => this.ChangeImage());
            IsNewCustomer = isNewCustomer;
        }

        /// <summary>
        /// Change the image of the customer. The image must be a .png, .jpg, or .jpeg file.
        /// </summary>
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
