using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public static class EntityExtensions
    {
        public static FeatureDetailSetting GetProtoFeatureDetailSetting(this PluginFeature.Models.DeviceFeatureSetting deviceFeatureSetting)
        {
            return new FeatureDetailSetting
            {
                Data = deviceFeatureSetting.Data,
                FeatureId = deviceFeatureSetting.FeatureId.ToString()
            };
        }

        public static FeatureDetailProfile GetProtoFeatureDetailProfile(this PluginFeature.Models.DeviceFeatureProfil deviceFeatureProfil)
        {
            return new FeatureDetailProfile
            {
                FeatureId = deviceFeatureProfil.FeatureId.ToString(),
                DisplayName = deviceFeatureProfil.DisplayName,
                Id = deviceFeatureProfil.Id,
                ProfileData = deviceFeatureProfil.ProfileData
            };
        }
    }
}
