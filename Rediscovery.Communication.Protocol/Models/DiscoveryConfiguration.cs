using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DiscoveryConfiguration : BaseConfiguration<ConnectionConfigurationDiscovery>
    {
        public const int DefaultListenPort = 13570;
        public const int DefaultSendPort = 13570;

        public bool ListenerDeactivated { get; set; }
        public bool SenderDeactivated { get; set; }

        public DiscoveryConfiguration()
        {
            Connection = new ConnectionConfigurationDiscovery(new List<int> { DefaultListenPort }, new List<int> { DefaultSendPort }, DefaultPackageSize);
        }
    }
}
