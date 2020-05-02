using IPCPipe.Models;
using Rediscovery.Desktop.Hub.Feature.RemoteResource;
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
        private readonly IDesktopHubRemoteResourceService _desktopHubRemoteResourceService;

        public event EventHandler<List<DeviceInfo>> DeviceInfoReceived;
        public event EventHandler<List<DeviceInfo>> ActiveDeviceInfoReceived;

        public List<DeviceInfo> Items { get; set; } = new List<DeviceInfo>();
        public List<DeviceInfo> ItemsActiveDeviceInfo { get; set; } = new List<DeviceInfo>();

        public DeviceService(IDesktopHubRemoteResourceService desktopHubRemoteResourceService)
        {
            _desktopHubRemoteResourceService = desktopHubRemoteResourceService;
            _desktopHubRemoteResourceService.ActiveDeviceInfoReceived += _desktopHubRemoteResourceService_ActiveDeviceInfoReceived;
            _desktopHubRemoteResourceService.DeviceInfoReceived += _desktopHubRemoteResourceService_DeviceInfoReceived;
        }

        private void _desktopHubRemoteResourceService_DeviceInfoReceived(object sender, List<DeviceInfo> e)
        {
            Items.Clear();
            Items.AddRange(e);
            DeviceInfoReceived?.Invoke(this, e);
        }

        private void _desktopHubRemoteResourceService_ActiveDeviceInfoReceived(object sender, List<DeviceInfo> e)
        {
            ItemsActiveDeviceInfo.Clear();
            ItemsActiveDeviceInfo.AddRange(e);
            ActiveDeviceInfoReceived?.Invoke(this, e);
        }

        public void Init()
        {
            _desktopHubRemoteResourceService.Connect();
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
    }
}
