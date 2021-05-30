using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DiscoveryConfiguration : BaseConfiguration
    {
        public const int DefaultListenPortDiscovery = 13570;
        public const int DefaultSendPortDiscovery = 13570;

        public bool ListenerDeactivated { get; set; }
        public bool SenderDeactivated { get; set; }

        public DiscoveryConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPortDiscovery, DefaultSendPortDiscovery, DefaultPackageSize);
        }
    }
}
