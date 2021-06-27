using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class LargeConfiguration : BaseConfiguration<ConnectionConfiguration>
    {
        public const int DefaultListenPort = 13572;
        public const int DefaultSendPort = 13572;

        public LargeConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPort, DefaultSendPort, DefaultPackageSize * 60);
        }
    }
}
