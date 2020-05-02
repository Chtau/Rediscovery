using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.RemoteResource
{
    public interface IDesktopHubRemoteResourceService
    {
        Task Connect();
        event EventHandler<List<SharedCoreModels.DeviceInfo>> ActiveDeviceInfoReceived;
        event EventHandler<List<SharedCoreModels.DeviceInfo>> DeviceInfoReceived;
        event EventHandler<List<SharedCoreModels.DeviceFeature>> ServiceFeatureReceived;
        event EventHandler<LoggerEntryModel> LogEntryReceived;
    }
}
