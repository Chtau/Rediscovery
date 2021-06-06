using Rediscovery.Communication.Protocol.Internal.Listener;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal interface IDeviceManager
    {
        event EventHandler<string> DeviceChanged;
        DeviceGreetingReceived GetGreeting(string identifier);
    }
}
