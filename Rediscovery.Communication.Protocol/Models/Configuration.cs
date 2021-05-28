using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class Configuration
    {
        public DiscoveryConfiguration Discovery { get; set; }
        public LowDataConfiguration LowData { get; set; }
        public DataConfiguration Data { get; set; }
    }
}
