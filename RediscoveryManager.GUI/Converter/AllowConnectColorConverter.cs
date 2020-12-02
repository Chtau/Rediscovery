using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.Converter
{
    public class AllowConnectColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Rediscovery.Shared.Base.Connection.Enums.AllowConnect)value)
            {
                case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.None:
                    return Brushes.Black;
                case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.OK:
                    return Brushes.Green;
                case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.Error:
                    return Brushes.Red;
                case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.Denied:
                    return Brushes.Red;
                case Rediscovery.Shared.Base.Connection.Enums.AllowConnect.UnkownDevice:
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
