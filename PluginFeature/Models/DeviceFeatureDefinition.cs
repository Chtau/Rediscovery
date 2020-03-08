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

        public object SettingsObject { get; set; }

        /// <summary>
        /// Profiles are in JSON format because of a serialize problem with SignalR
        /// </summary>
        public string Profiles { get; set; }

        public object UIZipArchive { get; set; }
    }
}
