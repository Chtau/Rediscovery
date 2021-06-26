using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class HandshakeConfiguration : BaseConfiguration
    {
        public const int DefaultListenPort = 13561;
        public const int DefaultSendPort = 13561;

        public HandshakeConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPort, DefaultSendPort, DefaultPackageSize);
        }
    }
}
