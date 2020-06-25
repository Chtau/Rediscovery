using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IResourcesRepository
    {
        List<SharedBase.Device.FeatureDefinitionExtended> GetResourceDeviceFeature();
        List<SharedBase.Device.DeviceInfo> GetResourceDeviceInfo();
        List<SharedBase.Device.DeviceInfo> GetResourceActiveDeviceInfo();
        bool DeleteDeviceInfo(Guid id);
        SharedBase.Device.DeviceInfo UpdateDeviceInfo(SharedBase.Device.DeviceInfo deviceInfo);
        List<SharedBase.Device.DeviceInfo> GetResourcePendingAuthenticationDevices();
        bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept);
        List<FeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId);
        FeatureSetting GetResourceDeviceFeatureSettings(Guid featureId);
    }
}
