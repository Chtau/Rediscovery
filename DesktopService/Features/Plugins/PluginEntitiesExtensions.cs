using Microsoft.Net.Http.Headers;
using Rediscovery.Shared.Base.Feature;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Plugins
{
    public static class PluginEntitiesExtensions
    {
        public static Rediscovery.Shared.Base.Device.FeatureDefinitionExtended GetFeatureDefinitionExtended(this Rediscovery.Feature.Plugin.Models.PluginFeatureDefinition entity)
        {
            return new Rediscovery.Shared.Base.Device.FeatureDefinitionExtended
            {
                Author = entity.Author,
                ControlIntegrationPoint = entity.ControlIntegrationPoint.GetIntegrationPoint(),
                DisplayName = entity.DisplayName,
                Documentation = entity.Documentation,
                FeatureIntegrationPoint = entity.FeatureIntegrationPoint.GetIntegrationPoint(),
                Id = entity.Id,
                MinimalControlIntegrationPoint = entity.MinimalControlIntegrationPoint.GetVersion(),
                MinimalFeatureIntegrationPoint = entity.MinimalFeatureIntegrationPoint.GetVersion(),
                PluginDirectory = entity.PluginDirectory,
                HasProfilConfiguration = entity.HasProfilConfiguration,
                HasSettingConfiguration = entity.HasSettingConfiguration,
                Version = entity.Version.GetVersion(),
                Website = entity.Website,
                IsClientImplementation = false,
                ClientDescription = null,
                NativeResources = 0
            };
        }

        public static Rediscovery.Shared.Base.Device.FeatureDefinitionExtended GetFeatureDefinitionExtended(this Rediscovery.Feature.Plugin.Models.PluginFeatureDefinitionClient entity)
        {
            return new Rediscovery.Shared.Base.Device.FeatureDefinitionExtended
            {
                Author = entity.Author,
                ControlIntegrationPoint = entity.ControlIntegrationPoint.GetIntegrationPoint(),
                DisplayName = entity.DisplayName,
                Documentation = entity.Documentation,
                FeatureIntegrationPoint = entity.FeatureIntegrationPoint.GetIntegrationPoint(),
                Id = entity.Id,
                MinimalControlIntegrationPoint = entity.MinimalControlIntegrationPoint.GetVersion(),
                MinimalFeatureIntegrationPoint = entity.MinimalFeatureIntegrationPoint.GetVersion(),
                PluginDirectory = entity.PluginDirectory,
                HasProfilConfiguration = entity.HasProfilConfiguration,
                HasSettingConfiguration = entity.HasSettingConfiguration,
                Version = entity.Version.GetVersion(),
                Website = entity.Website,
                IsClientImplementation = true,
                ClientDescription = entity.ClientDescription,
                NativeResources = (int)entity.NativeResources
            };
        }

        public static Rediscovery.Shared.Base.Device.IntegrationPoint GetIntegrationPoint(this Rediscovery.Feature.Plugin.Enums.PluginIntegration pluginIntegration)
        {
            switch (pluginIntegration)
            {
                case Rediscovery.Feature.Plugin.Enums.PluginIntegration.Desktop:
                    return Rediscovery.Shared.Base.Device.IntegrationPoint.Desktop;
                case Rediscovery.Feature.Plugin.Enums.PluginIntegration.Mobile:
                    return Rediscovery.Shared.Base.Device.IntegrationPoint.Mobile;
                default:
                    return Rediscovery.Shared.Base.Device.IntegrationPoint.Desktop;
            }
        }

        public static Rediscovery.Shared.Base.Core.Version GetVersion(this Rediscovery.Feature.Plugin.Models.PluginVersion pluginVersion)
        {
            return new Rediscovery.Shared.Base.Core.Version
            {
                Label = pluginVersion.Label,
                Major = pluginVersion.Major,
                Minor = pluginVersion.Minor,
                Patch = pluginVersion.Patch
            };
        }

        public static List<FeatureProfil> GetFeatureProfils(this List<Rediscovery.Feature.Plugin.Models.PluginFeatureProfil> profils)
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

        public static FeatureProfil GetFeatureProfil(this Rediscovery.Feature.Plugin.Models.PluginFeatureProfil profil)
        {
            return new FeatureProfil
            {
                DisplayName = profil.DisplayName,
                FeatureId = profil.FeatureId,
                Id = profil.Id,
                ProfileData = profil.ProfileData
            };
        }

        public static Rediscovery.Feature.Plugin.Models.PluginFeatureProfil GetPluginFeatureProfil(this FeatureProfil profil)
        {
            return new Rediscovery.Feature.Plugin.Models.PluginFeatureProfil
            {
                DisplayName = profil.DisplayName,
                FeatureId = profil.FeatureId,
                Id = profil.Id,
                ProfileData = profil.ProfileData
            };
        }

        public static FeatureSetting GetFeatureSetting(this Rediscovery.Feature.Plugin.Models.PluginFeatureSetting setting)
        {
            return new FeatureSetting
            {
                Data = setting.Data,
                FeatureId = setting.FeatureId
            };
        }

        public static Rediscovery.Feature.Plugin.Models.PluginFeatureSetting GetPluginFeatureSetting(this FeatureSetting setting)
        {
            return new Rediscovery.Feature.Plugin.Models.PluginFeatureSetting
            {
                Data = setting.Data,
                FeatureId = setting.FeatureId
            };
        }

        public static Rediscovery.Feature.Plugin.Models.PluginExchangeEntity<Rediscovery.Feature.Plugin.Models.PluginFeatureData> GetPluginExchangeEntity(this ExchangeEntity<FeatureData> exchangeEntity)
        {
            return new Rediscovery.Feature.Plugin.Models.PluginExchangeEntity<Rediscovery.Feature.Plugin.Models.PluginFeatureData>
            {
                Entity = exchangeEntity.Entity?.GetPluginFeatureData(),
                Sid = exchangeEntity.Sid
            };
        }

        public static Rediscovery.Feature.Plugin.Models.PluginExchangeEntity<Rediscovery.Feature.Plugin.Models.PluginFeatureDataClient> GetPluginExchangeEntityClient(this ExchangeEntity<FeatureData> exchangeEntity)
        {
            return new Rediscovery.Feature.Plugin.Models.PluginExchangeEntity<Rediscovery.Feature.Plugin.Models.PluginFeatureDataClient>
            {
                Entity = exchangeEntity.Entity?.GetPluginFeatureDataClient(),
                Sid = exchangeEntity.Sid
            };
        }

        public static ExchangeEntity<FeatureData> GetExchangeEntity(this Rediscovery.Feature.Plugin.Models.PluginExchangeEntity<Rediscovery.Feature.Plugin.Models.PluginFeatureData> exchangeEntity)
        {
            return new ExchangeEntity<FeatureData>
            {
                Entity = exchangeEntity.Entity?.GetFeatureData(),
                Sid = exchangeEntity.Sid
            };
        }

        public static ExchangeEntity<FeatureData> GetExchangeEntity(this Rediscovery.Feature.Plugin.Models.PluginExchangeEntity<Rediscovery.Feature.Plugin.Models.PluginFeatureDataClient> exchangeEntity)
        {
            return new ExchangeEntity<FeatureData>
            {
                Entity = exchangeEntity.Entity?.GetFeatureData(),
                Sid = exchangeEntity.Sid
            };
        }

        public static Rediscovery.Feature.Plugin.Models.PluginFeatureData GetPluginFeatureData(this FeatureData featureData)
        {
            return new Rediscovery.Feature.Plugin.Models.PluginFeatureData(featureData.DeviceId, featureData.FeatureId, featureData.ProfileId, featureData.Data);
        }

        public static Rediscovery.Feature.Plugin.Models.PluginFeatureDataClient GetPluginFeatureDataClient(this FeatureData featureData)
        {
            return new Rediscovery.Feature.Plugin.Models.PluginFeatureDataClient(featureData.DeviceId, featureData.FeatureId, featureData.ProfileId, featureData.Data, (Rediscovery.Feature.Plugin.Enums.ClientNativeResources)featureData.NativeResourceType);
        }

        public static FeatureData GetFeatureData(this Rediscovery.Feature.Plugin.Models.PluginFeatureData featureData)
        {
            return new FeatureData(featureData.DeviceId, featureData.FeatureId, featureData.ProfileId, featureData.Data);
        }

        public static FeatureData GetFeatureData(this Rediscovery.Feature.Plugin.Models.PluginFeatureDataClient featureDataClient)
        {
            return new FeatureData(featureDataClient.DeviceId, featureDataClient.FeatureId, featureDataClient.ProfileId, featureDataClient.Data, true, (int)featureDataClient.NativeResourceType);
        }
    }
}
