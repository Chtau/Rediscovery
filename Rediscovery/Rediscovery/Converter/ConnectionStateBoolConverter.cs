using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Converter
{
    public class ConnectionStateBoolConverter : IValueConverter
    {
        internal ILogger _logger => DependencyService.Get<ILogger>() ?? new Logger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (SharedCoreModels.Enums.ConnectionState)(int)value;
                return valueState == SharedCoreModels.Enums.ConnectionState.OK;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectionStateReverseBoolConverter : IValueConverter
    {
        internal ILogger _logger => DependencyService.Get<ILogger>() ?? new Logger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (SharedCoreModels.Enums.ConnectionState)(int)value;
                return !(valueState == SharedCoreModels.Enums.ConnectionState.OK);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return !false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
