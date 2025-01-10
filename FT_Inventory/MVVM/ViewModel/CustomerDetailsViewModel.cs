using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.Core;
using System.Collections.ObjectModel;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    class CustomerDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        public Customer CurrentCustomer { get; set; }
        public bool IsNewCustomer { get; }
        public CustomerDetailsViewModel(DatabaseManager dbManager, Customer currentCustomer, bool isNewCustomer)
        {
            _dbManager = dbManager;
            CurrentCustomer = currentCustomer;
            IsNewCustomer = isNewCustomer;
        }
    }
}
