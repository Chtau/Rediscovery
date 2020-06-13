using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Plugins
{
    public static class PluginEntitiesExtensions
    {
        public static SharedBase.Device.FeatureDefinitionExtended GetFeatureDefinitionExtended(this PluginFeature.Models.PluginFeatureDefinition entity)
        {
            return new SharedBase.Device.FeatureDefinitionExtended
            {
                Author = entity.Author,
                ControlIntegrationPoint = entity.ControlIntegrationPoint.GetIntegrationPoint(),
                DisplayName = entity.DisplayName,
                Documentation = entity.Documentation,
                FeatureIntegrationPoint = entity.ControlIntegrationPoint.GetIntegrationPoint(),
                HasProfiles = entity.HasProfiles,
                HasSettings = entity.HasSettings,
                Id = entity.Id,
                MinimalControlIntegrationPoint = entity.MinimalControlIntegrationPoint.GetVersion(),
                MinimalFeatureIntegrationPoint = entity.MinimalFeatureIntegrationPoint.GetVersion(),
                PluginDirectory = entity.PluginDirectory,
                ProfileUIElementName = entity.ProfileUIElementName,
                ProfileUIReadonly = entity.ProfileUIReadonly,
                SettingUIElementName = entity.SettingUIElementName,
                SettingUIReadonly = entity.SettingUIReadonly,
                Version = entity.Version.GetVersion(),
                Website = entity.Website
            };
        }

        public static SharedBase.Device.IntegrationPoint GetIntegrationPoint(this PluginFeature.Models.PluginFeatureDefinition.PluginIntegration pluginIntegration)
        {
            switch (pluginIntegration)
            {
                case PluginFeature.Models.PluginFeatureDefinition.PluginIntegration.Desktop:
                    return SharedBase.Device.IntegrationPoint.Desktop;
                case PluginFeature.Models.PluginFeatureDefinition.PluginIntegration.Mobile:
                    return SharedBase.Device.IntegrationPoint.Mobile;
                default:
                    return SharedBase.Device.IntegrationPoint.Desktop;
            }
        }

        public static SharedBase.Core.Version GetVersion(this PluginFeature.Models.PluginVersion pluginVersion)
        {
            return new SharedBase.Core.Version
            {
                Label = pluginVersion.Label,
                Major = pluginVersion.Major,
                Minor = pluginVersion.Minor,
                Patch = pluginVersion.Patch
            };
        }

        public static List<FeatureProfil> GetFeatureProfils(this List<PluginFeature.Models.PluginFeatureProfil> profils)
        {
            if (profils?.Count > 0)
            {
                var profilslist = new List<FeatureProfil>();
                foreach (var item in profils)
                {
                    profilslist.Add(item.GetFeatureProfil());
                }
                return profilslist;
            }
            return null;
        }

        public static FeatureProfil GetFeatureProfil(this PluginFeature.Models.PluginFeatureProfil profil)
        {
            return new FeatureProfil
            {
                DisplayName = profil.DisplayName,
                FeatureId = profil.FeatureId,
                Id = profil.Id,
                ProfileData = profil.ProfileData
            };
        }

        public static PluginFeature.Models.PluginFeatureProfil GetPluginFeatureProfil(this FeatureProfil profil)
        {
            return new PluginFeature.Models.PluginFeatureProfil
            {
                DisplayName = profil.DisplayName,
                FeatureId = profil.FeatureId,
                Id = profil.Id,
                ProfileData = profil.ProfileData
            };
        }

        public static FeatureSetting GetFeatureSetting(this PluginFeature.Models.PluginFeatureSetting setting)
        {
            return new FeatureSetting
            {
                Data = setting.Data,
                FeatureId = setting.FeatureId
            };
        }

        public static PluginFeature.Models.PluginFeatureSetting GetPluginFeatureSetting(this FeatureSetting setting)
        {
            return new PluginFeature.Models.PluginFeatureSetting
            {
                Data = setting.Data,
                FeatureId = setting.FeatureId
            };
        }

        public static PluginFeature.Models.PluginExchangeEntity<PluginFeature.Models.PluginFeatureData> GetPluginExchangeEntity(this ExchangeEntity<FeatureData> exchangeEntity)
        {
            return new PluginFeature.Models.PluginExchangeEntity<PluginFeature.Models.PluginFeatureData>
            {
                Entity = exchangeEntity.Entity?.GetPluginFeatureData(),
                Sid = exchangeEntity.Sid
            };
        }

        public static PluginFeature.Models.PluginFeatureData GetPluginFeatureData(this FeatureData featureData)
        {
            return new PluginFeature.Models.PluginFeatureData(featureData.DeviceId, featureData.FeatureId, featureData.ProfileId, featureData.Data);
        }

        public static ExchangeEntity<FeatureData> GetExchangeEntity(this PluginFeature.Models.PluginExchangeEntity<PluginFeature.Models.PluginFeatureData> exchangeEntity)
        {
            return new ExchangeEntity<FeatureData>
            {
                Entity = exchangeEntity.Entity?.GetFeatureData(),
                Sid = exchangeEntity.Sid
            };
        }

        public static FeatureData GetFeatureData(this PluginFeature.Models.PluginFeatureData featureData)
        {
            return new FeatureData(featureData.DeviceId, featureData.FeatureId, featureData.ProfileId, featureData.Data);
        }
    }
}
