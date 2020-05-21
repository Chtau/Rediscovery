using PluginFeature.Models;
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
        void UpdateDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo);
        List<SharedCoreModels.DeviceInfo> GetResourcePendingAuthenticationDevices();
        bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept);
        List<DeviceFeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId);
        DeviceFeatureSetting GetResourceDeviceFeatureSettings(Guid featureId);
        byte[] GetResourceDeviceFeatureSettingsUI(Guid featureId);
    }
}
