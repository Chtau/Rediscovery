using System;
using System.Collections.Generic;
using System.Text;

public static class EntityExtensions
{
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

    public static PluginFeature.Models.DeviceFeatureSetting GetDeviceFeatureSetting(this FeatureDetailSetting featureDetailSetting)
    {
        return new PluginFeature.Models.DeviceFeatureSetting
        {
            Data = featureDetailSetting.Data,
            FeatureId = featureDetailSetting.FeatureId.SafeGuid()
        };
    }

    public static FeatureDetailSetting GetProtoFeatureDetailSetting(this PluginFeature.Models.DeviceFeatureSetting deviceFeatureSetting)
    {
        return new FeatureDetailSetting
        {
            Data = deviceFeatureSetting.Data,
            FeatureId = deviceFeatureSetting.FeatureId.ToString()
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

    public static SharedBase.Device.DeviceInfo GetDeviceInfo(this DeviceInfo deviceInfo)
    {
        return new SharedBase.Device.DeviceInfo
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

    public static DeviceInfo GetProtoDeviceInfo(this SharedBase.Device.DeviceInfo deviceInfo)
    {
        return new DeviceInfo
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
}
