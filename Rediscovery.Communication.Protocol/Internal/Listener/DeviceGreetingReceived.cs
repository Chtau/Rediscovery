using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DeviceGreetingReceived
    {
        public DateTime Received { get; }
        public DeviceGreeting Device { get; }

        public DeviceGreetingReceived(DeviceGreeting device)
        {
            Device = device;
            Received = DateTime.UtcNow;
        }
    }
}
