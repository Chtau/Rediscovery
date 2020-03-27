using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Device
{
    public interface IDeviceService
    {
        List<DeviceInfo> Items { get; set; }
        void Refresh();
        void RemoveItem(DeviceInfo item);
    }
}
