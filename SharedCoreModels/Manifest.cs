using System;
using System.Collections.Generic;

namespace SharedCoreModels
{
    public class Manifest
    {
        public string ClientName { get; set; }

        public List<PluginFeature.Models.DeviceFeatureDefinition> SupportedFeatures { get; set; }

        public PluginFeature.Models.Version ClientVersion { get; set; }

        public PluginFeature.Models.Version AppMinimumVersion { get; set; }
    }
}
