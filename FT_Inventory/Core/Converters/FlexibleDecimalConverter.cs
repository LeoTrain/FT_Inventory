using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FT_Inventory.Core.Converters
{
    public class FlexibleDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d) return d.ToString("N2", CultureInfo.CurrentCulture);
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input) { input = input.Replace(',', '.'); if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result)) return result; }
            return 0m;
        }
    }
}
