using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
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
        private readonly Shared.ISharedEvents _sharedEvents;
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

        public SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration ConnectionConfiguration { get; set; }

        private SharedBase.Connection.Enums.AllowConnect allowConnect;
        public SharedBase.Connection.Enums.AllowConnect AllowConnect
        {
            get { return allowConnect; }
            set
            {
                this.RaiseAndSetIfChanged(ref allowConnect, value);
            }
        }

        private SharedBase.Connection.Enums.ConnectionState state;
        public SharedBase.Connection.Enums.ConnectionState State
        {
            get { return state; }
            set
            {
                this.RaiseAndSetIfChanged(ref state, value);
                switch (state)
                {
                    case SharedBase.Connection.Enums.ConnectionState.None:
                    case SharedBase.Connection.Enums.ConnectionState.Error:
                    case SharedBase.Connection.Enums.ConnectionState.Warning:
                    case SharedBase.Connection.Enums.ConnectionState.Offline:
                    case SharedBase.Connection.Enums.ConnectionState.Denied:
                    case SharedBase.Connection.Enums.ConnectionState.WaitForApprovel:
                        CanDisconnect = false;
                        CanConnect = true;
                        break;
                    case SharedBase.Connection.Enums.ConnectionState.OK:
                        CanDisconnect = true;
                        CanConnect = false;
                        break;
                    default:
                        break;
                }
            }
        }
        private bool canConnect;
        public bool CanConnect 
        { 
            get { return canConnect; }
            set
            {
                this.RaiseAndSetIfChanged(ref canConnect, value);
            }
        }
        private bool canDisconnect;
        public bool CanDisconnect 
        { 
            get { return canDisconnect; }
            set
            {
                this.RaiseAndSetIfChanged(ref canDisconnect, value);
            }
        }

        public StatusViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();
            ConnectionConfiguration = Locator.Current.GetService<SharedConfigurations.RediscoveryManager.GUI.Models.ConnectionConfiguration>();

            _manager.AfterConnecting += (obj, args) =>
            {
                AllowConnect = _manager.ManagerConnectionState.CanConnect;
                State = _manager.ManagerConnectionState.ConnectionState;
            };
            State = _manager.ManagerConnectionState.ConnectionState;
            if (!string.IsNullOrWhiteSpace(ConnectionConfiguration.IP) && (ConnectionConfiguration.Port > 0) && !string.IsNullOrWhiteSpace(ConnectionConfiguration.DeviceIdentifier))
            {
                CanConnect = true;
                CanDisconnect = false;
            } else
            {
                CanConnect = false;
                CanDisconnect = false;
            }
            if (ConnectionConfiguration.AutoConnect)
            {
                Connect();
            }
        }

        public void Connect()
        {
            try
            {
                _sharedEvents.InvokeLoading(this, true);
                CanConnect = false;
                CanDisconnect = false;
                Task.Run(() =>
                {
                    _manager.SetConnectionValues(ConnectionConfiguration.IP, ConnectionConfiguration.Port, ConnectionConfiguration.DeviceIdentifier);
                    _manager.TryConnect();
                    State = _manager.ManagerConnectionState.ConnectionState;
                    AllowConnect = _manager.ManagerConnectionState.CanConnect;
                    _sharedEvents.InvokeLoading(this, false);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Disconnect()
        {
            try
            {
                _manager.Disconnect();
                State = _manager.ManagerConnectionState.ConnectionState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
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
                        Disconnect();
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
