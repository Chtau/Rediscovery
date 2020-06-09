using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureConsumer
{
    public static class EntityExtensions
    {
        public static PluginFeature.Models.DeviceFeatureSetting GetDeviceFeatureSetting(this FeatureDetailSetting featureDetailSetting)
        {
            return new PluginFeature.Models.DeviceFeatureSetting
            {
                Data = featureDetailSetting.Data,
                FeatureId = featureDetailSetting.FeatureId.SafeGuid()
            };
        }

        public static PluginFeature.Models.DeviceFeatureProfil GetDeviceFeatureProfil(this FeatureDetailProfile featureDetailProfile)
        {
            return new PluginFeature.Models.DeviceFeatureProfil
            {
                DisplayName = featureDetailProfile.DisplayName,
                FeatureId = featureDetailProfile.FeatureId.SafeGuid(),
                Id = featureDetailProfile.Id,
                ProfileData = featureDetailProfile.ProfileData
            };
        }
    }
}
