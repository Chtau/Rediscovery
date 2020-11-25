using Rediscovery.Shared.Base.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Consumer.Authentication
{
    public interface IGreetingConsumerService
    {
        GreetingDeviceReply GreetHost(string host, int port, GreetingDeviceMessage greetingDevice, int secondsTimeout = 2);
        bool Disconnect();
    }
}
