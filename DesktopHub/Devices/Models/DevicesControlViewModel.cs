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
        private readonly IPCPipe.IPipeClient _pipeClient;

        public ObservableCollection<SharedCoreModels.DeviceInfo> Items { get; set; } = new ObservableCollection<DeviceInfo>();

        public DevicesControlViewModel()
        {
            _resourceProvider = (IPCPipe.IPipeResourceProvider)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeResourceProvider));
            _pipeClient = (IPCPipe.IPipeClient)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeClient));
        }

        public void Refresh()
        {
            _resourceProvider.Receiver<List<SharedCoreModels.DeviceInfo>>("rediscoveryservice", "deviceinfo", OnReceiveResource);
        }

        public void RemoveItem(DeviceInfo item)
        {
            if (item != null)
            {
                var sync = new IPCPipe.Models.Sync<DeviceInfo>
                {
                    ActionType = SyncAction.Delete,
                    Entity = item
                };
                _pipeClient.Send("sync_device_rediscoveryservice", Newtonsoft.Json.JsonConvert.SerializeObject(sync));
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
