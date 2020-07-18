using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Converter
{
    public class IntMinusEmptyConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int val && val >= 0)
            {
                return val;
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valString = value?.ToString();
            if (!string.IsNullOrWhiteSpace(valString) && int.TryParse(valString, out int ret))
            {
                return ret;
            }
            return -1;
        }
    }
}
