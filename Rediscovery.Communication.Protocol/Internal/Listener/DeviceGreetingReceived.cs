using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DeviceGreetingReceived
    {
        public string IP { get; private set; }
        public DateTime Received { get; private set; }
        public DeviceGreeting Device { get; private set; }

        public DeviceGreetingReceived(DeviceGreeting device, string ip)
        {
            Device = device;
            Received = DateTime.UtcNow;
            IP = ip;
        }

        /// <summary>
        /// Updates the greeting device information and the time received
        /// </summary>
        /// <param name="device">New received device greeting</param>
        /// <param name="ip">IP of the peer</param>
        /// <returns>true if device data has changed</returns>
        public bool Update(DeviceGreeting device, string ip)
        {
            if (device.Hops <= Device.Hops)
            {
                Device = device;
                Received = DateTime.UtcNow;
                IP = ip;
                return true;
            }

            return false;
        }
    }
}
