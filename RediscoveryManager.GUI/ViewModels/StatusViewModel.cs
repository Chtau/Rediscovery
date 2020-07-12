using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using RediscoveryManager.Service;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.ViewModels
{
    public class StatusViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly SharedBase.Logging.ILogger _logger;

        public SharedBase.Connection.Enums.ConnectionState State { get; set; }
        public bool CanConnect { get; set; }
        public bool CanDisconnect { get; set; }

        public StatusViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();

            _manager.AfterConnecting += (obj, args) =>
            {
                State = _manager.ManagerConnectionState.ConnectionState;
            };
            State = _manager.ManagerConnectionState.ConnectionState;
        }

        public void Connect()
        {

        }

        public void Disconnect()
        {

        }

        public void EditConnection()
        {
            try
            {
                var model = new ConnectionConfigurationViewModel
                {
                    DeviceIdentifier = null,
                    IPAddress = null,
                    Port = null
                };
                var configDialog = new Windows.ConnectionConfiguration(model);
                var desktopApp = (IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime;
                configDialog.ShowDialog(desktopApp.MainWindow);
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
