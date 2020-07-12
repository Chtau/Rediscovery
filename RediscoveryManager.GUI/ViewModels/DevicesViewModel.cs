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
    public class DevicesViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly SharedBase.Logging.ILogger _logger;
        private readonly Shared.ISharedEvents _sharedEvents;

        private ObservableCollection<DeviceInfoViewModelExtension> items;
        public ObservableCollection<DeviceInfoViewModelExtension> Items 
        { 
            get { return items; }
            set
            {
                this.RaiseAndSetIfChanged(ref items, value);
            }
        }

        public DevicesViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();
            _sharedEvents = Locator.Current.GetService<Shared.ISharedEvents>();

            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                var newCollection = new List<DeviceInfoViewModelExtension>();
                foreach (var item in _manager.Devices)
                {
                    var newItem = new DeviceInfoViewModelExtension(item);
                    newCollection.Add(newItem);
                }
                Items = new ObservableCollection<DeviceInfoViewModelExtension>(newCollection);
            };
        }

        public void DeleteDevice(SharedBase.Device.DeviceInfo deviceInfo)
        {
            // TODO: add callback for the user control view model
            System.Diagnostics.Debug.Print("Delete device");
        }

        
    }
}
