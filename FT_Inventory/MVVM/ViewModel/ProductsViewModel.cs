using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FT_Inventory.Core;
using FT_Inventory.MVVM.Models;
using System.Printing;
using System.Windows.Navigation;
using FT_Inventory.Core.Commands;
using FT_Inventory.Core.Exceptions;

namespace FT_Inventory.MVVM.ViewModel
{
    public class ProductsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        private string _selectedCategory;
        private string _searchText;
        public Product SelectedProduct { get; set; }
        public string[] Categories { get; set; }
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    this.UpdateProductsBySearchText();
                }
            }
        }
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                this.UpdateProductsByCategory();
            }
        }
        public ObservableCollection<Product> Products { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }

        public ProductsViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            SearchText = "";
            LoadProducts();
            DeleteProductCommand = new RelayCommand(o =>
            {
                if (SelectedProduct != null)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this product?",
                                                              "Delete Confirmation",
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _dbManager.DeleteProduct(SelectedProduct);
                        LoadProducts();
                    }
                }
                else
                    MessageBox.Show("Please select a product to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public void LoadProducts()
        {
            try
            {
                if (this._dbManager.IsConnected)
                {
                    List<Product> productsFromDb = this._dbManager.GetAllProducts();
                    this.Categories = this._dbManager.GetAllProductCategories();
                    this.SelectedCategory = Categories[0];
                    this.SelectedProduct = productsFromDb[0];
                    this.Products = new ObservableCollection<Product>(productsFromDb);
                }
                else
                {
                    this.Categories = new string[] { "No Categories" };
                    this.SelectedCategory = Categories[0];
                    this.SelectedProduct = null;
                    this.Products = new ObservableCollection<Product>();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No Products found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.SelectedCategory = "No Categories";
                this.SelectedProduct = null;
                this.Products = new ObservableCollection<Product>();
            }

        }

        private void UpdateProductsByCategory()
        {
            if (SelectedCategory == "All")
            {
                Products = new ObservableCollection<Product>(_dbManager.GetAllProducts());
                OnPropertyChanged(nameof(Products));
                SelectedProduct = Products[0];
                return;
            }
            var productsFromDb = _dbManager.GetProductsByCategory(SelectedCategory);
            Products = new ObservableCollection<Product>(productsFromDb);
            OnPropertyChanged(nameof(Products));
            if (Products.Count > 0)
                SelectedProduct = Products[0];
        }

        private void UpdateProductsBySearchText()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                Products = new ObservableCollection<Product>(_dbManager.GetAllProducts());
            else
            {
                var productsFromDb = _dbManager.GetAllProducts();
                var filteredProducts = productsFromDb
                    .Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Products = new ObservableCollection<Product>(filteredProducts);
            }
            OnPropertyChanged(nameof(Products));
        }
    }
}
