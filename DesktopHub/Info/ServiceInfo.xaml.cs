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
        }

        public ServiceInfo(SharedConfigurations.Hub.Models.ServiceInfoConfiguration serviceInfoConfiguration) : this()
        {
            DataContext = viewModel = new ServiceInfoViewModel()
            {
                IpAddr = serviceInfoConfiguration.IP + ":" + serviceInfoConfiguration.Port
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
