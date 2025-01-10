using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    internal class OrderDetailsViewModel : ViewModelBase
    {
        private DatabaseManager _dbManager;
        public Order CurrentOrder { get; set; }
        public bool IsNewOrder { get; }
        public OrderDetailsViewModel(DatabaseManager dbManager, Order currentOrder, bool isNewOrder)
        {
            _dbManager = dbManager;
            CurrentOrder = currentOrder;
            IsNewOrder = isNewOrder;
        }
    }
}
