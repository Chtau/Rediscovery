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
    }
}
