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
                    DeviceIdentifier = null,
                    IPAddress = null,
                    Port = null
                };
                var configDialog = new Windows.ConnectionConfiguration(model);
                var desktopApp = (IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime;
                var result = await configDialog.ShowDialog<bool>(desktopApp.MainWindow);
                if (result)
                {
                    if (!string.IsNullOrWhiteSpace(model.IPAddress) && (model.Port.HasValue && model.Port.Value > 0) && !string.IsNullOrWhiteSpace(model.DeviceIdentifier))
                    {
                        _manager.SetConnectionValues(model.IPAddress, model.Port.Value, model.DeviceIdentifier);
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
