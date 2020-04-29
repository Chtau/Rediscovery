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
        private readonly IPCPipe.IPipeServer _pipeServer;

        public event EventHandler<List<DeviceInfo>> DeviceInfoReceived;
        public event EventHandler<List<DeviceInfo>> ActiveDeviceInfoReceived;

        public List<DeviceInfo> Items { get; set; } = new List<DeviceInfo>();
        public List<DeviceInfo> ItemsActiveDeviceInfo { get; set; } = new List<DeviceInfo>();

        public DeviceService(IPCPipe.IPipeResourceProvider pipeResourceProvider,
            IPCPipe.IPipeServer pipeServer)
        {
            _resourceProvider = pipeResourceProvider;
            _pipeServer = pipeServer;
        }

        public void Init()
        {
            _resourceProvider.Receiver<List<DeviceInfo>>("rediscoveryservice", "deviceinfo", OnReceiveResource);
            _resourceProvider.Receiver<List<DeviceInfo>>("rediscoveryservice", "activedeviceinfo", OnReceiveResourceActiveDevice);
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
                //_pipeClient.Send("sync_device_rediscoveryservice", Newtonsoft.Json.JsonConvert.SerializeObject(sync));
                Items.Remove(item);
            }
        }

        private void OnReceiveResource(PipeResource<List<DeviceInfo>> resource)
        {
            Items.Clear();
            Items.AddRange(resource.Entity);
            DeviceInfoReceived?.Invoke(this, resource.Entity);
        }

        private void OnReceiveResourceActiveDevice(PipeResource<List<DeviceInfo>> resource)
        {
            ItemsActiveDeviceInfo.Clear();
            ItemsActiveDeviceInfo.AddRange(resource.Entity);
            ActiveDeviceInfoReceived?.Invoke(this, resource.Entity);
        }
    }
}
