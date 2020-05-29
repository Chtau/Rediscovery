using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Device
{
    public class FeatureDefinitionExtended : FeatureDefinition, IFeatureMetadataDefinition, 
        IFeatureProfileDefinition, IFeatureSettingDefinition, IFeatureIntegrationDefinition
    {
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
        public IntegrationPoint ControlIntegrationPoint { get; set; }
        public IntegrationPoint FeatureIntegrationPoint { get; set; }
    }
}
