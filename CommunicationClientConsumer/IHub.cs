using CommunicationBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationClientConsumer
{
    public interface IHub
    {
        void Init(ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP);
        void Authenticate(string deviceIdentifier, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback);
        void Connect(string deviceIdentifier, ConnectionConfiguration configuration, Action<bool> listenerCallback);
        void Disconnect();
    }
}
