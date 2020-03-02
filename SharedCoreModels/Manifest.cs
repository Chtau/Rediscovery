using System;
using System.Collections.Generic;

namespace SharedCoreModels
{
    public class Manifest
    {
        public string ClientName { get; set; }

        public List<DeviceFeature.DeviceFeatureDefinition> SupportedFeatures { get; set; }

        public Version ClientVersion { get; set; }

        public Version AppMinimumVersion { get; set; }
    }
}
