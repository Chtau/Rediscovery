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
        public const int DefaultPackageBytes = 1024;

        public int ListenPortDiscovery { get; set; } = DefaultListenPortDiscovery;
        public long ListenPackageBytesDiscovery { get; set; } = DefaultPackageBytes;
        public long SendPackageBytesDiscovery { get; set; } = DefaultPackageBytes;
        public int ListenPortData { get; set; } = DefaultListenPortData;
        public long ListenPackageBytesData { get; set; } = DefaultPackageBytes;
        public long SendPackageBytesData { get; set; } = DefaultPackageBytes;
        public int ListenPortLowData { get; set; } = DefaultListenPortLowData;
        public long ListenPackageBytesLowData { get; set; } = DefaultPackageBytes;
        public long SendPackageBytesLowData { get; set; } = DefaultPackageBytes;
    }
}
