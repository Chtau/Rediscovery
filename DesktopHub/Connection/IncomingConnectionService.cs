using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopHub.Connection
{
    public class IncomingConnectionService : IIncomingConnectionService
    {
        private readonly Connection.IIncomingConnectionPipe _incomingConnectionPipe;
        private readonly Connection.IIncomingConnectionPipeLiveLogger _incomingConnectionPipeLiveLogger;
        private List<IncomingConnection> incomingConnectionsWindows = new List<IncomingConnection>();

        public IncomingConnectionService()
        {
            _incomingConnectionPipe = (Connection.IIncomingConnectionPipe)Program.ServiceProvider.GetService(typeof(Connection.IIncomingConnectionPipe));
            _incomingConnectionPipe.NewConnectionInfo += _incomingConnectionPipe_NewConnectionInfo;
            _incomingConnectionPipeLiveLogger = (Connection.IIncomingConnectionPipeLiveLogger)Program.ServiceProvider.GetService(typeof(Connection.IIncomingConnectionPipeLiveLogger));
            _incomingConnectionPipeLiveLogger.LiveLoggerEntry += _incomingConnectionPipeLiveLogger_LiveLoggerEntry;
        }

        public void Init()
        {
            _incomingConnectionPipe.ListenForConnections();
            _incomingConnectionPipeLiveLogger.ListenForConnections();
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
                var index = OnGetWindowIndex(model);
                if (index != -1)
                {
                    incomingConnectionsWindows[index].UpdateModel(e.Code, e.ValidTill);
                    incomingConnectionsWindows[index].Focus();
                }
                else
                {
                    var conWindow = new Connection.IncomingConnection(model);
                    conWindow.Closed += ConWindow_Closed;
                    conWindow.Show();
                    incomingConnectionsWindows.Add(conWindow);
                }
            });
        }

        private void _incomingConnectionPipeLiveLogger_LiveLoggerEntry(object sender, SharedCoreModels.LiveLoggerModel e)
        {
            
        }

        private void ConWindow_Closed(object sender, EventArgs e)
        {
            var window = (IncomingConnection)sender;
            var index = OnGetWindowIndex(window.Model);
            if (index != -1)
            {
                incomingConnectionsWindows.RemoveAt(index);
            }
        }

        private int OnGetWindowIndex(Models.IncomingConnectionViewModel model)
        {
            return incomingConnectionsWindows.FindIndex(x => string.Equals(x.Model.Device, model.Device, StringComparison.OrdinalIgnoreCase));
        }
    }
}