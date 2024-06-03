using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ModBusTPU.Models.ModbusSensor;
using ModBusTPU.ViewModels;
using ModBusTPU.ViewModels.ChartViewModels;

namespace ModBusTPU.Converter
{
    public class ModbusSensorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ModbusSensorViewModel((ModbusSensor)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
