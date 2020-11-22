using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.Discovery.Models
{
    public class DiscoveryConfiguration
    {
        public const string SectionName = "Discovery";

        public ushort Port { get; set; }

        public string FirewallRuleName { get; set; }
    }
}
