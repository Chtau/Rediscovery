using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Devices
{
    public class DevicesControl : UserControl
    {
        private readonly Models.DevicesControlViewModel _viewModel;

        public DevicesControl()
        {
            this.InitializeComponent();

            DataContext = _viewModel = new Models.DevicesControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
