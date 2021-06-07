using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Device
{
    internal interface IDeviceManager
    {
        List<DeviceGreeting> Devices { get; }

        /// <summary>
        /// Will be triggered after a device is changed or added
        /// </summary>
        event EventHandler<string> DeviceChanged;
        /// <summary>
        /// Get the current device greeting information from the device identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        DeviceGreetingReceived GetGreeting(string identifier);
        /// <summary>
        /// Add or update device greeting information.
        /// Timeout check will be started if no timeout is running.
        /// </summary>
        /// <param name="deviceGreeting"></param>
        /// <param name="ipEndPoint"></param>
        /// <returns>True if this is a new device or something has changed. False when no changes happend.</returns>
        bool Change(DeviceGreeting deviceGreeting, IPEndPoint ipEndPoint);
        /// <summary>
        /// Set the identifier of the current device
        /// </summary>
        /// <param name="identifer"></param>
        void SetIdentifier(string identifer);
    }
}
