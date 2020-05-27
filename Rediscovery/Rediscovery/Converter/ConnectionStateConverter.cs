using Rediscovery.Services;
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
                var valueState = (SharedCoreModels.Enums.ConnectionState)(int)value;
                string resultText = "";
                switch (valueState)
                {
                    case SharedCoreModels.Enums.ConnectionState.None:
                        resultText = "";
                        break;
                    case SharedCoreModels.Enums.ConnectionState.OK:
                        resultText = "Connected";
                        break;
                    case SharedCoreModels.Enums.ConnectionState.Error:
                        resultText = "Connection error";
                        break;
                    case SharedCoreModels.Enums.ConnectionState.Warning:
                        resultText = "Connection warning";
                        break;
                    case SharedCoreModels.Enums.ConnectionState.Offline:
                        resultText = "Offline";
                        break;
                    case SharedCoreModels.Enums.ConnectionState.Denied:
                        resultText = "Access denied";
                        break;
                    case SharedCoreModels.Enums.ConnectionState.WaitForApprovel:
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
