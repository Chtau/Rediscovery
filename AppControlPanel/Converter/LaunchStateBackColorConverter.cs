using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppControlPanel.Converter
{
    public class LaunchStateBackColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse<ViewModels.AppViewModel.LaunchState>(value?.ToString(), out ViewModels.AppViewModel.LaunchState state))
            {
                switch (state)
                {
                    case ViewModels.AppViewModel.LaunchState.None:
                        return Brushes.White;
                    case ViewModels.AppViewModel.LaunchState.Running:
                        return Brushes.Green;
                    case ViewModels.AppViewModel.LaunchState.Error:
                        return Brushes.Red;
                    default:
                        return Brushes.White;
                }
            } else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
