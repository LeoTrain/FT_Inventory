using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FT_Inventory.Core;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;
using Microsoft.Win32;

namespace FT_Inventory.MVVM.ViewModel
{
    class ProductDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        public Product CurrentProduct { get; set; }
        public bool IsNewProduct { get; }
        public string CurrencySymbol { get; }
        public RelayCommand ChangeImageCommand { get; }
        public RelayCommand AddCategoryCommand { get; }
        public List<string> Categories { get; set; }

        public ProductDetailsViewModel(DatabaseManager dbManager, Product currentProduct, bool isNewProduct)
        {
            _dbManager = dbManager;
            CurrentProduct = currentProduct;
            IsNewProduct = isNewProduct;
            CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            Categories = _dbManager.GetAllProductCategories().ToList();
            ChangeImageCommand = new RelayCommand(o => this.ChangeImage());
            AddCategoryCommand = new RelayCommand(o => this.AddCategory());
        }

        private void AddCategory()
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("Enter a new category: ", "Add Category");
            if (!string.IsNullOrWhiteSpace(input) && !Categories.Contains(input))
            {
                Categories.Add(input);
                CurrentProduct.Category = input;
                OnPropertyChanged(nameof(Categories));
                OnPropertyChanged(nameof(CurrentProduct));
            }
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
                CurrentProduct.ImageUrl = openFileDialog.FileName;
                OnPropertyChanged(nameof(CurrentProduct));
            }
        }

    }
}
