using System;
using System.Collections.Generic;
using System.Text;
using static SharedCoreModels.Enums;

namespace SharedCoreModels.DesktopPlugins
{
    public interface IDesktopPluginFeatureDefinition
    {
        IntegrationPoint FeatureIntegrationPoint { get; set; }
        IntegrationPoint ControlIntegrationPoint { get; set; }
        ControlIntegrationType ControlIntegration { get; set; }
        string DisplayName { get; set; }
        Guid Id { get; set; }
        Version Version { get; set; }
        Version MinFeatureIntegrationPoint { get; set; }
        Version MinControlIntegrationPoint { get; set; }
        object SettingsObject { get; set; }
    }
}
