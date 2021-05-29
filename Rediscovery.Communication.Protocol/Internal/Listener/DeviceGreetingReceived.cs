using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DeviceGreetingReceived
    {
        public DateTime Received { get; private set; }
        public DeviceGreeting Device { get; private set; }

        public DeviceGreetingReceived(DeviceGreeting device)
        {
            Device = device;
            Received = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the greeting device information and the time received
        /// </summary>
        /// <param name="device">New received device greeting</param>
        /// <returns>true if device data has changed</returns>
        public bool Update(DeviceGreeting device)
        {
            Device = device;
            Received = DateTime.UtcNow;

            return false;
        }
    }
}
