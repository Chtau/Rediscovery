using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public class DeviceFeature
    {
        public enum IntegrationPoint
        {
            Desktop = 0,
            Mobile = 1
        }

        public enum ControlIntegrationType
        {
            None,
            Terminal,
            MediaPlayer
        }

        public IntegrationPoint FeatureIntegrationPoint { get; set; } = IntegrationPoint.Desktop;
        public IntegrationPoint ControlIntegrationPoint { get; set; } = IntegrationPoint.Mobile;
        public ControlIntegrationType ControlIntegration { get; set; } = ControlIntegrationType.Terminal;
        public string DisplayName { get; set; }
        public Guid Id { get; set; }
        public Version Version { get; set; }
        public Version MinFeatureIntegrationPoint { get; set; }
        public Version MinControlIntegrationPoint { get; set; }
        public object SettingsObject { get; set; }
    }
}
