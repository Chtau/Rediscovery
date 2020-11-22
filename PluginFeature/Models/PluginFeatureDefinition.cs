using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Models
{
    public class PluginFeatureDefinition
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public PluginVersion Version { get; set; }

        public PluginVersion MinimalFeatureIntegrationPoint { get; set; }

        public PluginVersion MinimalControlIntegrationPoint { get; set; }

        public string Author { get; set; }

        public string Documentation { get; set; }

        public string Website { get; set; }

        public string PluginDirectory { get; set; }

        public bool HasProfilConfiguration { get; set; }

        public bool HasSettingConfiguration { get; set; }

        public Enums.PluginIntegration ControlIntegrationPoint { get; set; }

        public Enums.PluginIntegration FeatureIntegrationPoint { get; set; }

        public string DesktopExecutable { get; set; }
    }
}
