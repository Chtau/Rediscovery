using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesRepository
    {
        void SendActiveDeviceInfo();
        void SendDeviceInfo();
        void SendServiceFeature();
        void DeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo);
    }
}
