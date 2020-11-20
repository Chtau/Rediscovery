using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Converter
{
    public abstract class BaseValueConverter : IValueConverter
    {
        internal SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Services.Logger();

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
