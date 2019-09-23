using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Connection
{
    public class IncomingConnection : Window
    {
        private readonly Connection.Models.IncomingConnectionViewModel _viewModel;

        public IncomingConnection(Connection.Models.IncomingConnectionViewModel viewModel)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = _viewModel = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
