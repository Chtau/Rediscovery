using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DeviceGreeting
    {
        public string Identifier { get; set; }
        public string FriendlyName { get; set; }
        public DeviceCommunication Communication { get; set; }
        public DeviceMetadata Metadata { get; set; }
    }
}
