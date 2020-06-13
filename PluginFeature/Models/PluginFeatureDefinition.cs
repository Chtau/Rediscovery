using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Models
{
    public class PluginFeatureDefinition
    {
        public enum PluginIntegration
        {
            Desktop = 0,
            Mobile = 1
        }

        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public PluginVersion Version { get; set; }

        public PluginVersion MinimalFeatureIntegrationPoint { get; set; }

        public PluginVersion MinimalControlIntegrationPoint { get; set; }

        public string Author { get; set; }

        public string Documentation { get; set; }

        public string Website { get; set; }

        public string PluginDirectory { get; set; }

        public bool HasProfiles { get; set; }

        public bool ProfileUIReadonly { get; set; }

        public string ProfileUIElementName { get; set; }

        public bool HasSettings { get; set; }

        public bool SettingUIReadonly { get; set; }

        public string SettingUIElementName { get; set; }

        public PluginIntegration ControlIntegrationPoint { get; set; }

        public PluginIntegration FeatureIntegrationPoint { get; set; }
    }
}
