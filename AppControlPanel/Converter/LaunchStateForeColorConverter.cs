using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppControlPanel.Converter
{
    public class LaunchStateForeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse<ViewModels.AppViewModel.LaunchState>(value?.ToString(), out ViewModels.AppViewModel.LaunchState state))
            {
                switch (state)
                {
                    case ViewModels.AppViewModel.LaunchState.None:
                        return Brushes.Black;
                    case ViewModels.AppViewModel.LaunchState.Running:
                        return Brushes.White;
                    case ViewModels.AppViewModel.LaunchState.Error:
                        return Brushes.White;
                    default:
                        return Brushes.Black;
                }
            } else
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
