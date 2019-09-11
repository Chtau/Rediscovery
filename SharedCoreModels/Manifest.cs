using System;
using System.Collections.Generic;

namespace SharedCoreModels
{
    public class Manifest
    {
        public string ClientName { get; set; }

        public List<string> SupportedFeatures { get; set; }

        public Version ClientVersion { get; set; }

        public Version AppMinimumVersion { get; set; }
    }
}
