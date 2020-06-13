using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationConsumer
{
    public interface IGreetingConsumerService
    {
        GreetingDeviceReply GreetHost(string host, GreetingDeviceMessage greetingDevice);
    }
}
