using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FT_Inventory.Core;
using FT_Inventory.Core.Commands;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    class ProductDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        public Product CurrentProduct { get; set; }
        public bool IsNewProduct { get; }

        public ProductDetailsViewModel(DatabaseManager dbManager, Product currentProduct, bool isNewProduct)
        {
            _dbManager = dbManager;
            CurrentProduct = currentProduct;
            IsNewProduct = isNewProduct;

        }

    }
}
