using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer
{
    public static class EntityExtensions
    {
        public static SharedCoreModels.DeviceInfo GetDeviceInfo(this Resources.DeviceInfo deviceInfo)
        {
            return new SharedCoreModels.DeviceInfo
            {
                AllowAccess = deviceInfo.AllowAccess,
                DeviceType = deviceInfo.DeviceType,
                Id = deviceInfo.Id.SafeGuid(),
                Identifier = deviceInfo.Identifier,
                Idiom = deviceInfo.Idiom,
                Manufacturer = deviceInfo.Manufacturer,
                Model = deviceInfo.Model,
                Name = deviceInfo.Name,
                OSVersion = deviceInfo.OSVersion,
                Platform = deviceInfo.Platform,
                RequestTime = deviceInfo.RequestTime.TicksLongDatetime(),
            };
        }

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
                RequestTime = deviceInfo.RequestTime.DatetimeTicksLong()
            };
        }

        public static SharedBase.Device.FeatureDefinitionExtended GetFeatureDefinition(this FeatureDefinitionExtended featureDefinitionExtended)
        {
            return new SharedBase.Device.FeatureDefinitionExtended
            {
                Author = featureDefinitionExtended.Author,
                ControlIntegrationPoint = (SharedBase.Device.IntegrationPoint)(int)featureDefinitionExtended.ControlIntegrationPoint,
                DisplayName = featureDefinitionExtended.DisplayName,
                Documentation = featureDefinitionExtended.Documentation,
                FeatureIntegrationPoint = (SharedBase.Device.IntegrationPoint)(int)featureDefinitionExtended.FeatureIntegrationPoint,
                HasProfiles = featureDefinitionExtended.HasProfiles,
                HasSettings = featureDefinitionExtended.HasSettings,
                Id = featureDefinitionExtended.Id.SafeGuid(),
                MinimalControlIntegrationPoint = SharedBase.Core.Version.ConvertTo(featureDefinitionExtended.MinimalControlIntegrationPoint),
                MinimalFeatureIntegrationPoint = SharedBase.Core.Version.ConvertTo(featureDefinitionExtended.MinimalFeatureIntegrationPoint),
                PluginDirectory = featureDefinitionExtended.PluginDirectory,
                ProfileUIElementName = featureDefinitionExtended.ProfileUIElementName,
                ProfileUIReadonly = featureDefinitionExtended.ProfileUIReadonly,
                SettingUIElementName = featureDefinitionExtended.SettingUIElementName,
                SettingUIReadonly = featureDefinitionExtended.SettingUIReadonly,
                Version = SharedBase.Core.Version.ConvertTo(featureDefinitionExtended.Version),
                Website = featureDefinitionExtended.Website,
            };
        }
    }
}
