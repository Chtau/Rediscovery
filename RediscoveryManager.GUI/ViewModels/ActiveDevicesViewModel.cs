using ReactiveUI;
using Rediscovery.Client.App.Manager.GUI.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.ViewModels
{
    public class ActiveDevicesViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly Rediscovery.Shared.Logging.ILogger _logger;
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

        public ActiveDevicesViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<Rediscovery.Shared.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();

            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                var newCollection = new List<DeviceInfoViewModelExtension>();
                foreach (var item in _manager.ActiveDevices)
                {
                    var newItem = new DeviceInfoViewModelExtension(item, null);
                    newCollection.Add(newItem);
                }
                Items = new ObservableCollection<DeviceInfoViewModelExtension>(newCollection);
            };
        }
    }
}
