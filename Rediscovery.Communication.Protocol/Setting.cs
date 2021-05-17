using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class Setting
    {
        public const int DefaultListenPortDiscovery = 13570;
        public const int DefaultListenPortData = 13571;
        public const int DefaultListenPortLowData = 13572;

        public int ListenPortDiscovery { get; set; } = DefaultListenPortDiscovery;
        public int ListenPortData { get; set; } = DefaultListenPortData;
        public int ListenPortLowData { get; set; } = DefaultListenPortLowData;
    }
}
