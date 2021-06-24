using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class LargeConfiguration : BaseConfiguration
    {
        public const int DefaultListenPortLarge = 13572;
        public const int DefaultSendPortLarge = 13572;

        public LargeConfiguration()
        {
            Connection = new ConnectionConfiguration(DefaultListenPortLarge, DefaultSendPortLarge, DefaultPackageSize * 60);
        }
    }
}
