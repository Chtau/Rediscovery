using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace DesktopHub
{
    public class MainWindow : Window
    {
        private readonly Connection.IIncomingConnectionPipe _incomingConnectionPipe;

        public MainWindow()
        {
            InitializeComponent();

            _incomingConnectionPipe = (Connection.IIncomingConnectionPipe)Program.ServiceProvider.GetService(typeof(Connection.IIncomingConnectionPipe));
            _incomingConnectionPipe.NewConnectionInfo += _incomingConnectionPipe_NewConnectionInfo;
            _incomingConnectionPipe.ListenForConnections();
        }

        private void _incomingConnectionPipe_NewConnectionInfo(object sender, SharedCoreModels.IncomingConnectionInfo e)
        {
            System.Diagnostics.Debug.Print("New Connection infos");
            var model = new Connection.Models.IncomingConnectionViewModel(null);
            model.Code = e.Code;
            model.Device = e.Device;
            model.InitCountdown(e.ValidTill);
            Dispatcher.UIThread.Post(() =>
            {
                var conWindow = new Connection.IncomingConnection(model);
                conWindow.Show();
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}