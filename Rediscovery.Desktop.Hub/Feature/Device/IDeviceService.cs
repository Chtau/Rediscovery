using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Device
{
    public interface IDeviceService
    {
        event EventHandler<List<DeviceInfo>> DeviceInfoReceived;
        event EventHandler<List<DeviceInfo>> ActiveDeviceInfoReceived;
        List<DeviceInfo> Items { get; set; }
        List<DeviceInfo> ItemsActiveDeviceInfo { get; set; }
        void Init();
        void RemoveItem(DeviceInfo item);
    }
}
