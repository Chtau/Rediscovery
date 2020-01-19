using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Connection
{
    public class IncomingConnection : Window
    {
        private readonly Connection.Models.IncomingConnectionViewModel _viewModel;

        public Connection.Models.IncomingConnectionViewModel Model
        {
            get { return _viewModel; }
        }

        public IncomingConnection()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public IncomingConnection(Connection.Models.IncomingConnectionViewModel viewModel) : this()
        {
            DataContext = _viewModel = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void UpdateModel(string code, System.DateTime validTill)
        {
            _viewModel.Code = code;
            _viewModel.InitCountdown(validTill);
        }
    }
}
