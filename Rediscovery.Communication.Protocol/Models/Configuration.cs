using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class Configuration
    {
        public DiscoveryConfiguration Discovery { get; set; } = new DiscoveryConfiguration();
        public DataConfiguration Data { get; set; } = new DataConfiguration();
    }
}
