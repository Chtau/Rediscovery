using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using IPCPipe.Models;
using SharedCoreModels;
using System.Linq;
using Avalonia.Threading;

namespace DesktopHub.Devices.Models
{
    public class DevicesControlViewModel : BaseViewModel
    {
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;

        public ObservableCollection<SharedCoreModels.DeviceInfo> Items { get; set; } = new ObservableCollection<DeviceInfo>();

        public DevicesControlViewModel()
        {
            _resourceProvider = (IPCPipe.IPipeResourceProvider)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeResourceProvider));
        }

        public void Refresh()
        {
            _resourceProvider.Receiver<List<SharedCoreModels.DeviceInfo>>("rediscoveryservice", "deviceinfo", OnReceiveResource);
        }

        public void RemoveItem(DeviceInfo item)
        {
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        private void OnReceiveResource(PipeResource<List<DeviceInfo>> resource)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Items.Clear();
                foreach (var item in resource.Entity)
                {
                    Items.Add(item);
                }
            });
        }
    }
}
