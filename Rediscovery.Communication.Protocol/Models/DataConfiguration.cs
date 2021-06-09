using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DataConfiguration : BaseConfiguration
    {
        public const int DefaultListenPortData = 13571;
        public const int DefaultSendPortData = 13571;

        public const int DefaultListenPortDataLarge = 13572;
        public const int DefaultSendPortDataLarge = 13572;

        public ConnectionConfiguration ConnectionLargeData { get; set; }

        public DataConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPortData, DefaultSendPortData, DefaultPackageSize);
            ConnectionLargeData = new ConnectionConfiguration(DefaultListenPortData, DefaultSendPortData, DefaultPackageSize * 60);
        }
    }
}
