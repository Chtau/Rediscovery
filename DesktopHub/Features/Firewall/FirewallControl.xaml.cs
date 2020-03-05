using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Features.Firewall
{
    public class FirewallControl : UserControl
    {
        private readonly FirewallControlViewModel _viewModel;

        public FirewallControl()
        {
            this.InitializeComponent();
            DataContext = _viewModel = new FirewallControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
