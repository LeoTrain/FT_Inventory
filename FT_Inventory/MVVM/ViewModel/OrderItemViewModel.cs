using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_Inventory.MVVM.Models;

namespace FT_Inventory.MVVM.ViewModel
{
    internal class OrderItemViewModel : ViewModelBase
    {
        private DatabaseManager dbManager;

        private OrderItem _currentOrderItem;
        public OrderItem CurrentOrderItem
        {
            get { return _currentOrderItem; }
            set
            {
                _currentOrderItem = value;
                OnPropertyChanged(nameof(CurrentOrderItem));
            }
        }
        public bool IsNewOrderItem { get; }
        public OrderItemViewModel(DatabaseManager _dbManager, OrderItem currentOrderItem, bool isNewOrderItem)
        {
            dbManager = _dbManager;
            _currentOrderItem = currentOrderItem;
            IsNewOrderItem = isNewOrderItem;
        }

    }
}
