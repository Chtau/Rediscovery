using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DeviceGreeting
    {
        /// <summary>
        /// Unique device identifier
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// User defined friendly name
        /// </summary>
        public string FriendlyName { get; set; }
        public DeviceCommunication Communication { get; set; }
        public DeviceMetadata Metadata { get; set; }
        /// <summary>
        /// Required peer hops to reach this device.
        /// If this is a direct peer then there is 0 hop
        /// </summary>
        public int Hops { get; set; } = 0;
    }
}
