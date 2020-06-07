using PluginFeature.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public interface IResourcesRepository
    {
        List<SharedBase.Device.FeatureDefinitionExtended> GetResourceDeviceFeature();
        List<SharedCoreModels.DeviceInfo> GetResourceDeviceInfo();
        List<SharedCoreModels.DeviceInfo> GetResourceActiveDeviceInfo();
        void DeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo);
        void UpdateDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo);
        List<DeviceInfo> GetResourcePendingAuthenticationDevices();
        bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept);
        List<DeviceFeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId);
        DeviceFeatureSetting GetResourceDeviceFeatureSettings(Guid featureId);
        byte[] GetResourceDeviceFeatureSettingsUI(Guid featureId);
        byte[] GetResourceDeviceFeatureProfilesUI(Guid featureId);
        bool SaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting);
        bool SaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil);
        bool DeleteFeatureProfile(Guid featureId, string profileId);
    }
}
