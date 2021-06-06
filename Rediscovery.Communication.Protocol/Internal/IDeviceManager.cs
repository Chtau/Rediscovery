using Rediscovery.Communication.Protocol.Internal.Listener;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal interface IDeviceManager
    {
        event EventHandler<string> DeviceChanged;
        DeviceGreetingReceived GetGreeting(string identifier);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceGreeting"></param>
        /// <param name="ipEndPoint"></param>
        /// <returns>True if this is a new device or something has changed. False when no changes happend.</returns>
        bool Change(DeviceGreeting deviceGreeting, IPEndPoint ipEndPoint);
    }
}
