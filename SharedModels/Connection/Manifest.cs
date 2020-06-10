using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Connection
{
    public class Manifest
    {
        public string ClientName { get; set; }

        public List<Device.FeatureDefinitionExtended> SupportedFeatures { get; set; }

        public Core.Version ClientVersion { get; set; }

        public Core.Version AppMinimumVersion { get; set; }
    }
}
