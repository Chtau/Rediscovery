using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public static class EntityExtensions
    {
        public static Resources.DeviceInfo GetProtoDeviceInfo(this SharedCoreModels.DeviceInfo deviceInfo)
        {
            return new Resources.DeviceInfo
            {
                AllowAccess = deviceInfo.AllowAccess,
                DeviceType = deviceInfo.DeviceType.EmptyIfNull(),
                Id = deviceInfo.Id.ToString(),
                Identifier = deviceInfo.Identifier.EmptyIfNull(),
                Idiom = deviceInfo.Idiom.EmptyIfNull(),
                Manufacturer = deviceInfo.Manufacturer.EmptyIfNull(),
                Model = deviceInfo.Model.EmptyIfNull(),
                Name = deviceInfo.Name.EmptyIfNull(),
                OSVersion = deviceInfo.OSVersion.EmptyIfNull(),
                Platform = deviceInfo.Platform.EmptyIfNull(),
                RequestTime = deviceInfo.RequestTime.HasValue ? (ulong)deviceInfo.RequestTime.Value.Ticks : 0
            };
        }

        public static FeatureDefinitionExtended GetProtoFeatureDefinition(this SharedBase.Device.FeatureDefinitionExtended featureDefinitionExtended)
        {
            return new FeatureDefinitionExtended
            {
                Author = featureDefinitionExtended.Author.EmptyIfNull(),
                ControlIntegrationPoint = (FeatureDefinitionExtended.Types.IntegrationPoint)(int)featureDefinitionExtended.ControlIntegrationPoint,
                DisplayName = featureDefinitionExtended.DisplayName.EmptyIfNull(),
                Documentation = featureDefinitionExtended.Documentation.EmptyIfNull(),
                FeatureIntegrationPoint = (FeatureDefinitionExtended.Types.IntegrationPoint)(int)featureDefinitionExtended.FeatureIntegrationPoint,
                HasProfiles = featureDefinitionExtended.HasProfiles,
                HasSettings = featureDefinitionExtended.HasSettings,
                Id = featureDefinitionExtended.Id.ToString(),
                MinimalControlIntegrationPoint = featureDefinitionExtended.MinimalControlIntegrationPoint.ToString(),
                MinimalFeatureIntegrationPoint = featureDefinitionExtended.MinimalFeatureIntegrationPoint.ToString(),
                PluginDirectory = featureDefinitionExtended.PluginDirectory.EmptyIfNull(),
                ProfileUIElementName = featureDefinitionExtended.ProfileUIElementName.EmptyIfNull(),
                ProfileUIReadonly = featureDefinitionExtended.ProfileUIReadonly,
                SettingUIElementName = featureDefinitionExtended.SettingUIElementName.EmptyIfNull(),
                SettingUIReadonly = featureDefinitionExtended.SettingUIReadonly,
                Version = featureDefinitionExtended.Version.ToString(),
                Website = featureDefinitionExtended.Website.EmptyIfNull()
            };
        }
    }
}
