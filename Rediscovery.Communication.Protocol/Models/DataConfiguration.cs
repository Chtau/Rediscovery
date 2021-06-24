using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DataConfiguration : BaseConfiguration
    {
        public const int DefaultListenPortData = 13571;
        public const int DefaultSendPortData = 13571;

        public DataConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPortData, DefaultSendPortData, DefaultPackageSize);
        }
    }
}
