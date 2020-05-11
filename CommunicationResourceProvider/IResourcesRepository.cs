using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IResourcesRepository
    {
        List<SharedCoreModels.DeviceFeature> GetResourceDeviceFeature();
        List<SharedCoreModels.DeviceInfo> GetResourceDeviceInfo();
        List<SharedCoreModels.DeviceInfo> GetResourceActiveDeviceInfo();
        void DeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo);
    }
}
