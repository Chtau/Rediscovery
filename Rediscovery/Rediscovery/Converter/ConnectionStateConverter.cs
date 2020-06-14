using Rediscovery.Services;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Converter
{
    public class ConnectionStateConverter : IValueConverter
    {
        internal SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Logger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (Enums.ConnectionState)(int)value;
                string resultText = "";
                switch (valueState)
                {
                    case Enums.ConnectionState.None:
                        resultText = "";
                        break;
                    case Enums.ConnectionState.OK:
                        resultText = "Connected";
                        break;
                    case Enums.ConnectionState.Error:
                        resultText = "Connection error";
                        break;
                    case Enums.ConnectionState.Warning:
                        resultText = "Connection warning";
                        break;
                    case Enums.ConnectionState.Offline:
                        resultText = "Offline";
                        break;
                    case Enums.ConnectionState.Denied:
                        resultText = "Access denied";
                        break;
                    case Enums.ConnectionState.WaitForApprovel:
                        resultText = "Wait for Approval";
                        break;
                    default:
                        resultText = "";
                        break;
                }
                return resultText;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
