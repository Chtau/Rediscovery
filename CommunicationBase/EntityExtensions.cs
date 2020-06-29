using SharedBase.Feature;
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
            Id = featureDefinitionExtended.Id.ToString(),
            MinimalControlIntegrationPoint = featureDefinitionExtended.MinimalControlIntegrationPoint.ToString(),
            MinimalFeatureIntegrationPoint = featureDefinitionExtended.MinimalFeatureIntegrationPoint.ToString(),
            PluginDirectory = featureDefinitionExtended.PluginDirectory.EmptyIfNull(),
            HasProfilConfiguration = featureDefinitionExtended.HasProfilConfiguration,
            HasSettingConfiguration = featureDefinitionExtended.HasSettingConfiguration,
            Version = featureDefinitionExtended.Version.ToString(),
            Website = featureDefinitionExtended.Website.EmptyIfNull()
        };
    }

    public static FeatureSetting GetDeviceFeatureSetting(this FeatureDetailSetting featureDetailSetting)
    {
        return new FeatureSetting
        {
            Data = featureDetailSetting.Data,
            FeatureId = featureDetailSetting.FeatureId.SafeGuid()
        };
    }

    public static FeatureDetailSetting GetProtoFeatureDetailSetting(this FeatureSetting deviceFeatureSetting)
    {
        return new FeatureDetailSetting
        {
            Data = deviceFeatureSetting.Data,
            FeatureId = deviceFeatureSetting.FeatureId.ToString()
        };
    }

    public static FeatureProfil GetDeviceFeatureProfil(this FeatureDetailProfile featureDetailProfile)
    {
        return new FeatureProfil
        {
            DisplayName = featureDetailProfile.DisplayName,
            FeatureId = featureDetailProfile.FeatureId.SafeGuid(),
            Id = featureDetailProfile.Id,
            ProfileData = featureDetailProfile.ProfileData
        };
    }

    public static FeatureDetailProfile GetProtoFeatureDetailProfile(this FeatureProfil deviceFeatureProfil)
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
            Id = featureDefinitionExtended.Id.SafeGuid(),
            MinimalControlIntegrationPoint = SharedBase.Core.Version.ConvertTo(featureDefinitionExtended.MinimalControlIntegrationPoint),
            MinimalFeatureIntegrationPoint = SharedBase.Core.Version.ConvertTo(featureDefinitionExtended.MinimalFeatureIntegrationPoint),
            PluginDirectory = featureDefinitionExtended.PluginDirectory,
            HasProfilConfiguration = featureDefinitionExtended.HasProfilConfiguration,
            HasSettingConfiguration = featureDefinitionExtended.HasSettingConfiguration,
            Version = SharedBase.Core.Version.ConvertTo(featureDefinitionExtended.Version),
            Website = featureDefinitionExtended.Website,
            NativeResources = featureDefinitionExtended.NativeResources,
            IsClientImplementation = featureDefinitionExtended.IsClientImplementation
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
