using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
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
            if (device.Hops < Device.Hops)
            {
                // peer connection with fewer hops
                Device = device;
                Received = DateTime.UtcNow;
                IP = ip;
                return true;
            } else if (device.Hops == Device.Hops)
            {
                // update for received time
                Device = device;
                Received = DateTime.UtcNow;
                IP = ip;
            }
            // peer connection with more hops are ignored because they are only be relevant
            // if the current device will be removed from the timeout
            return false;
        }
    }
}
