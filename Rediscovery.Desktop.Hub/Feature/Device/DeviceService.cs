using IPCPipe.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Device
{
    public class DeviceService : IDeviceService
    {
        private readonly IPCPipe.IPipeResourceProvider _resourceProvider;
        private readonly IPCPipe.IPipeClient _pipeClient;

        public event EventHandler<List<DeviceInfo>> DeviceInfoReceived;

        public List<DeviceInfo> Items { get; set; } = new List<DeviceInfo>();

        public DeviceService(IPCPipe.IPipeResourceProvider pipeResourceProvider,
            IPCPipe.IPipeClient pipeClient)
        {
            _resourceProvider = pipeResourceProvider;
            _pipeClient = pipeClient;
        }

        public void Refresh()
        {
            _resourceProvider.Receiver<List<DeviceInfo>>("rediscoveryservice", "deviceinfo", OnReceiveResource);
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
            Items.Clear();
            Items.AddRange(resource.Entity);
            DeviceInfoReceived?.Invoke(this, resource.Entity);
        }
    }
}
