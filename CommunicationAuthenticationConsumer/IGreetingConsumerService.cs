using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationConsumer
{
    public interface IGreetingConsumerService
    {
        string GreetHost(string host, string deviceIdentifier);
    }
}
