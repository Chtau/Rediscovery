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
        internal SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Logger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (SharedCoreModels.Enums.ConnectionState)(int)value;
                return valueState == SharedCoreModels.Enums.ConnectionState.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
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
        internal SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Logger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (SharedCoreModels.Enums.ConnectionState)(int)value;
                return !(valueState == SharedCoreModels.Enums.ConnectionState.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return !false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
