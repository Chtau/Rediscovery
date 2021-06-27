using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DataConfiguration : BaseConfiguration<ConnectionConfiguration>
    {
        public const int DefaultListenPort = 13571;
        public const int DefaultSendPort = 13571;

        public DataConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPort, DefaultSendPort, DefaultPackageSize);
        }
    }
}
