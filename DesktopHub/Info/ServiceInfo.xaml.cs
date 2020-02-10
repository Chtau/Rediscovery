using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Info
{
    public class ServiceInfo : Window
    {
        private ServiceInfoViewModel viewModel;

        public ServiceInfo()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = viewModel = new ServiceInfoViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
