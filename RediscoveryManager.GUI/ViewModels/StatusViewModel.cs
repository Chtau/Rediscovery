using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using RediscoveryManager.Service;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RediscoveryManager.GUI.ViewModels
{
    public class StatusViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly SharedBase.Logging.ILogger _logger;
        private Notifications.INotificationService _notification;

        public Notifications.INotificationService Notification
        {
            get
            {
                if (_notification == null)
                    _notification = Locator.Current.GetService<Notifications.INotificationService>();
                return _notification;
            }
        }

        //private SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration _connectionConfiguration;
        public SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration ConnectionConfiguration { get; set; }
        /*{
            get
            {
                if (_connectionConfiguration == null)
                    _connectionConfiguration = Locator.Current.GetService<SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration>();
                return _connectionConfiguration;
            }
            set
            {
                _connectionConfiguration = value;
            }
        }*/
        public SharedBase.Connection.Enums.ConnectionState State { get; set; }
        public bool CanConnect { get; set; }
        public bool CanDisconnect { get; set; }

        public StatusViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();
            ConnectionConfiguration = Locator.Current.GetService<SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration>();

            _manager.AfterConnecting += (obj, args) =>
            {
                State = _manager.ManagerConnectionState.ConnectionState;
            };
            State = _manager.ManagerConnectionState.ConnectionState;
        }

        public void Connect()
        {
            try
            {
                _manager.TryConnect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Disconnect()
        {

        }

        public async Task EditConnection()
        {
            try
            {
                var model = new ConnectionConfigurationViewModel
                {
                    DeviceIdentifier = ConnectionConfiguration.DeviceIdentifier,
                    IPAddress = ConnectionConfiguration.IP,
                    Port = ConnectionConfiguration.Port
                };
                var configDialog = new Windows.ConnectionConfiguration(model);
                var result = await configDialog.ShowDialog<bool>(Program.MainWindow);
                if (result)
                {
                    ConnectionConfiguration.DeviceIdentifier = model.DeviceIdentifier;
                    if (model.Port.HasValue)
                        ConnectionConfiguration.Port = model.Port.Value;
                    else
                        ConnectionConfiguration.Port = -1;
                    ConnectionConfiguration.IP = model.IPAddress;
                    if (!string.IsNullOrWhiteSpace(ConnectionConfiguration.IP) && (ConnectionConfiguration.Port > 0) && !string.IsNullOrWhiteSpace(ConnectionConfiguration.DeviceIdentifier))
                    {
                        _manager.SetConnectionValues(ConnectionConfiguration.IP, ConnectionConfiguration.Port, ConnectionConfiguration.DeviceIdentifier);
                        Connect();
                    } else
                    {
                        Notification.Show("Connection configuration", "Connection configuration is missing some Values");
                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
