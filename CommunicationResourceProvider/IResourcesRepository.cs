using PluginFeature.Models;
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
        List<DeviceFeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId);
        DeviceFeatureSetting GetResourceDeviceFeatureSettings(Guid featureId);
        byte[] GetResourceDeviceFeatureSettingsUI(Guid featureId);
        byte[] GetResourceDeviceFeatureProfilesUI(Guid featureId);
        bool SaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting);
        bool SaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil);
        bool DeleteFeatureProfile(Guid featureId, string profileId);
    }
}
