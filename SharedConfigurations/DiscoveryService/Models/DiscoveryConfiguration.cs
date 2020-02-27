using System;
using System.Collections.Generic;
using System.Text;

namespace SharedConfigurations.DiscoveryService.Models
{
    public class DiscoveryConfiguration
    {
        public ushort Port { get; set; }

        public string FirewallRuleName { get; set; }
    }
}
