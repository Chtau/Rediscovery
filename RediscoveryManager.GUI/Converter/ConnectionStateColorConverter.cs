using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.Converter
{
    public class ConnectionStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SharedBase.Connection.Enums.ConnectionState)value)
            {
                case SharedBase.Connection.Enums.ConnectionState.None:
                    return Brushes.Black;
                case SharedBase.Connection.Enums.ConnectionState.OK:
                    return Brushes.Green;
                case SharedBase.Connection.Enums.ConnectionState.Error:
                    return Brushes.Red;
                case SharedBase.Connection.Enums.ConnectionState.Warning:
                    return Brushes.Yellow;
                case SharedBase.Connection.Enums.ConnectionState.Offline:
                    return Brushes.White;
                case SharedBase.Connection.Enums.ConnectionState.Denied:
                    return Brushes.Red;
                case SharedBase.Connection.Enums.ConnectionState.WaitForApprovel:
                    return Brushes.Black;
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
