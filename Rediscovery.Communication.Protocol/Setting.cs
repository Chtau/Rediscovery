using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class Setting
    {
        public const int DefaultListenPortDiscovery = 13570;
        public const int DefaultSendPortDiscovery = 13570;
        public const int DefaultListenPortData = 13571;
        public const int DefaultSendPortData = 13571;
        public const int DefaultListenPortLowData = 13572;
        public const int DefaultSendPortLowData = 13572;
        public const int DefaultPackageBytes = 1024;

        public int ListenPortDiscovery { get; set; } = DefaultListenPortDiscovery;
        public int ListenPackageBytesDiscovery { get; set; } = DefaultPackageBytes;
        public int SendPortDiscovery { get; set; } = DefaultSendPortDiscovery;
        public int SendPackageBytesDiscovery { get; set; } = DefaultPackageBytes;
        public int ListenPortData { get; set; } = DefaultListenPortData;
        public int ListenPackageBytesData { get; set; } = DefaultPackageBytes;
        public int SendPortData { get; set; } = DefaultSendPortData;
        public int SendPackageBytesData { get; set; } = DefaultPackageBytes;
        public int ListenPortLowData { get; set; } = DefaultListenPortLowData;
        public int ListenPackageBytesLowData { get; set; } = DefaultPackageBytes;
        public int SendPortLowData { get; set; } = DefaultSendPortLowData;
        public int SendPackageBytesLowData { get; set; } = DefaultPackageBytes;
    }
}
