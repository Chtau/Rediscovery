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

        [Obsolete("We should provide the settings object a api similar to UI")]
        public object SettingsObject { get; set; }

        /// <summary>
        /// Profiles are in JSON format because of a serialize problem with SignalR
        /// </summary>
        [Obsolete("We should provide the settings object a api similar to UI")]
        public string Profiles { get; set; }
    }
}
