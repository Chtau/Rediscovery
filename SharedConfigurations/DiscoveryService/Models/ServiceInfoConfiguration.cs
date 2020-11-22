using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Configurations.Discovery.Models
{
    public class ServiceInfoConfiguration
    {
        public const string SectionName = "ServiceInfo";

        public ushort Port { get; set; }

        public string MetaInfo { get; set; }

        public string Name { get; set; }

        public string IP { get; set; }
    }
}
