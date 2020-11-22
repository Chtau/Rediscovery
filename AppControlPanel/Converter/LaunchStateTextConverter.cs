using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rediscovery.Client.App.ControlPanel.Converter
{
    public class LaunchStateTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse<ViewModels.AppViewModel.LaunchState>(value?.ToString(), out ViewModels.AppViewModel.LaunchState state))
            {
                switch (state)
                {
                    case ViewModels.AppViewModel.LaunchState.None:
                        return "";
                    case ViewModels.AppViewModel.LaunchState.Running:
                        return "running";
                    case ViewModels.AppViewModel.LaunchState.Error:
                        return "error";
                    case ViewModels.AppViewModel.LaunchState.NotFound:
                        return "not found";
                    case ViewModels.AppViewModel.LaunchState.Starting:
                        return "starting";
                    case ViewModels.AppViewModel.LaunchState.ErrorStarting:
                        return "error starting";
                    case ViewModels.AppViewModel.LaunchState.NotRunning:
                        return "not running";
                    default:
                        return "";
                }
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
