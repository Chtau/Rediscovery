using ReactiveUI;
using RediscoveryManager.GUI.Models;
using RediscoveryManager.Service;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RediscoveryManager.GUI.ViewModels
{
    public class PendingDevicesViewModel : ViewModelBase
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

        private ObservableCollection<DeviceInfoViewModelExtension> items;
        public ObservableCollection<DeviceInfoViewModelExtension> Items
        {
            get { return items; }
            set
            {
                this.RaiseAndSetIfChanged(ref items, value);
            }
        }

        public PendingDevicesViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();

            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                var newCollection = new List<DeviceInfoViewModelExtension>();
                foreach (var item in _manager.PendingDevices)
                {
                    var newItem = new DeviceInfoViewModelExtension(item, OnDeviceResolveCallback);
                    newCollection.Add(newItem);
                }
                Items = new ObservableCollection<DeviceInfoViewModelExtension>(newCollection);
            };
        }

        private void OnDeviceResolveCallback(DeviceInfoViewModelExtension item)
        {
            try
            {
                Notification.Show("Allow Device", $"The Device \"{item.Name}\" want to access the service, allow it ?", result =>
                {
                    _manager.TryResolvePendingDevice(item.Id, result);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
