using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class LowDataConfiguration : BaseConfiguration
    {
        public const int DefaultListenPortLowData = 13572;
        public const int DefaultSendPortLowData = 13572;

        public LowDataConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPortLowData, DefaultSendPortLowData, DefaultPackageSize);
        }
    }
}
