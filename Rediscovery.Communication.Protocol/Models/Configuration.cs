using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class Configuration
    {
        public DiscoveryConfiguration Discovery { get; set; } = new DiscoveryConfiguration();
        public LowDataConfiguration LowData { get; set; } = new LowDataConfiguration();
        public DataConfiguration Data { get; set; } = new DataConfiguration();
    }
}
