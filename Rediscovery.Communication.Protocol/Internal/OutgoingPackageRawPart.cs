using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class OutgoingPackageRawPart
    {
        public byte[] Raw { get; set; }
        public DeviceGreetingReceived DeviceGreeting { get; set; }
    }
}
