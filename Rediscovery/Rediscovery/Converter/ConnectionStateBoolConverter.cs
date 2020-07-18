using Rediscovery.Services;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Converter
{
    public class ConnectionStateBoolConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (Enums.ConnectionState)(int)value;
                return valueState == Enums.ConnectionState.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }
    }

    public class ConnectionStateReverseBoolConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (Enums.ConnectionState)(int)value;
                return !(valueState == Enums.ConnectionState.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return !false;
            }
        }
    }
}
