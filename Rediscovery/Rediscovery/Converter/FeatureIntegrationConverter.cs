using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Converter
{
    public class FeatureIntegrationConverter : BaseValueConverter
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valueState = (SharedBase.Device.IntegrationPoint)(int)value;
                if (parameter != null && int.TryParse(parameter.ToString(), out int val))
                {
                    var paramState = (SharedBase.Device.IntegrationPoint)val;
                    return valueState == paramState;
                } else
                {
                    return valueState == SharedBase.Device.IntegrationPoint.Desktop;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
