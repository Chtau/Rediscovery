using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class HandshakeConfiguration : BaseConfiguration
    {
        public const int DefaultListenPortData = 13561;
        public const int DefaultSendPortData = 13561;

        public HandshakeConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPortData, DefaultSendPortData, DefaultPackageSize);
        }
    }
}
