using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DeviceHandshake
    {
        public string IdentifierLocal { get; set; }
        public string IdentifierRemote { get; set; }
        public string ProposeIdentifier { get; set; }
        public DeviceCommunication Communication { get; set; }
        public bool Accepted { get; set; }
    }
}
