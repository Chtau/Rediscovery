using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace PluginFeature.Models
{
    public class DeviceFeatureDefinition
    {
        public IntegrationPoint FeatureIntegrationPoint { get; set; } = IntegrationPoint.Desktop;

        public IntegrationPoint ControlIntegrationPoint { get; set; } = IntegrationPoint.Mobile;

        public ControlIntegrationType ControlIntegration { get; set; } = ControlIntegrationType.Terminal;

        public string DisplayName { get; set; }

        public Guid Id { get; set; }

        public Models.Version Version { get; set; }

        public Models.Version MinFeatureIntegrationPoint { get; set; }

        public Models.Version MinControlIntegrationPoint { get; set; }

        public bool HasSettings { get; set; }

        public bool HasProfiles { get; set; }

        public string Author { get; set; }

        public string Documentation { get; set; }

        public string Url { get; set; }

        public string PluginDirectory { get; set; }

        public bool SettingsUIReadonly { get; set; }

        public bool ProfileUIReadonly { get; set; }
    }
}
