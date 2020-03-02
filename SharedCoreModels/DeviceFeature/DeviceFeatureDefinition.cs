using SharedCoreModels.DesktopPlugins;
using System;
using System.Collections.Generic;
using System.Text;
using static SharedCoreModels.Enums;

namespace SharedCoreModels.DeviceFeature
{
    public class DeviceFeatureDefinition
    {
        public IntegrationPoint FeatureIntegrationPoint { get; set; } = IntegrationPoint.Desktop;
        public IntegrationPoint ControlIntegrationPoint { get; set; } = IntegrationPoint.Mobile;
        public ControlIntegrationType ControlIntegration { get; set; } = ControlIntegrationType.Terminal;
        public string DisplayName { get; set; }
        public Guid Id { get; set; }
        public Version Version { get; set; }
        public Version MinFeatureIntegrationPoint { get; set; }
        public Version MinControlIntegrationPoint { get; set; }
        public object SettingsObject { get; set; }
        public List<DeviceFeatureProfil> Profiles { get; set; }
    }
}
