using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using IPCPipe.Models;
using SharedCoreModels;

namespace DesktopHub.Devices.Models
{
    public class DevicesControlViewModel : BaseViewModel
    {
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;

        public ObservableCollection<SharedCoreModels.DeviceInfo> Items { get; set; }

        public DevicesControlViewModel()
        {
            _resourceProvider = (IPCPipe.IPipeResourceProvider)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeResourceProvider));
        }

        public void Refresh()
        {
            _resourceProvider.Receiver<List<SharedCoreModels.DeviceInfo>>("rediscoveryservice", "deviceinfo", OnReceiveResource);
        }

        private void OnReceiveResource(PipeResource<List<DeviceInfo>> resource)
        {
            Items = new ObservableCollection<DeviceInfo>(resource.Entity);
        }
    }
}
