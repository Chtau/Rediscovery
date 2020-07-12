using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RediscoveryManager.GUI.Converter
{
    public class AllowConnectColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SharedBase.Connection.Enums.AllowConnect)value)
            {
                case SharedBase.Connection.Enums.AllowConnect.None:
                    return Brushes.Black;
                case SharedBase.Connection.Enums.AllowConnect.OK:
                    return Brushes.Green;
                case SharedBase.Connection.Enums.AllowConnect.Error:
                    return Brushes.Red;
                case SharedBase.Connection.Enums.AllowConnect.Denied:
                    return Brushes.Red;
                case SharedBase.Connection.Enums.AllowConnect.UnkownDevice:
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
