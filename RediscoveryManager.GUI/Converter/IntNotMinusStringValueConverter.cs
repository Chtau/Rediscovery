using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RediscoveryManager.GUI.Converter
{
    public class IntNotMinusStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int val && val >= 0)
            {
                return val.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int res))
            {
                if (res >= 0)
                    return res;
            }
            return -1;
        }
    }
}
