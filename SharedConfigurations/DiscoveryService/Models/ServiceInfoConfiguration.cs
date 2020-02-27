using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DiscoveryService.Models
{
    public class ServiceInfoConfiguration
    {
        public ushort Port { get; set; }

        public string MetaInfo { get; set; }

        public string Name { get; set; }

        public string IP { get; set; }
    }
}
