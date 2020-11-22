using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.Converter
{
    public class DateToTimeOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToLongTimeString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var dt = DateTime.UtcNow;
                if (TimeSpan.TryParse(value.ToString(), out TimeSpan ts))
                {
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds);
                }
                return dt;
            }
            return null;
        }
    }
}
