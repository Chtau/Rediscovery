using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Device
{
    public class FeatureDefinitionExtended : FeatureDefinition, IFeatureMetadataDefinition, 
        IFeatureProfileDefinition, IFeatureSettingDefinition, IFeatureIntegrationDefinition
    {
        public string Author { get; set; }
        public string Documentation { get; set; }
        public string Website { get; set; }
        public string PluginDirectory { get; set; }
        public bool HasProfilConfiguration { get; set; }
        public bool HasSettingConfiguration { get; set; }
        public IntegrationPoint ControlIntegrationPoint { get; set; }
        public IntegrationPoint FeatureIntegrationPoint { get; set; }
        public int NativeResources { get; set; }
        public bool IsClientImplementation { get; set; }
        public string ClientDescription { get; set; }
        public string DesktopExecutable { get; set; }
    }
}
