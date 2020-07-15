using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RediscoveryManager.GUI.Converter
{
    public class TimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                string retVal = "";
                if (timeSpan.Seconds > 0)
                    retVal = timeSpan.TotalSeconds.ToString("##0.00") + " sec ";
                else
                    retVal = timeSpan.TotalMilliseconds.ToString("##0.00") + " ms";
                return retVal;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrWhiteSpace(value?.ToString()))
            {
                var str = value.ToString();
                return str;
            }
            return null;
        }
    }
}
