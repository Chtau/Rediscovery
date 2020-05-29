using System;
using System.Collections.Generic;

namespace SharedCoreModels
{
    public class Manifest
    {
        public string ClientName { get; set; }

        public List<SharedBase.Device.FeatureDefinitionExtended> SupportedFeatures { get; set; }

        public SharedBase.Core.Version ClientVersion { get; set; }

        public SharedBase.Core.Version AppMinimumVersion { get; set; }
    }
}
