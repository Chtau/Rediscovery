using CommunicationBase;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationClientConsumer
{
    public interface IHub
    {
        void Init(ILogger logger, string authHubLink, string exchangeHubLink, Protocol protocol = Protocol.HTTP);
        void Authenticate(WelcomeDeviceMessage welcomeDeviceMessage, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback, Action<Manifest> manifestCallback);
        void Connect(string deviceIdentifier, ConnectionConfiguration configuration);
        void Disconnect();
        void Send(Guid featureId, string profileId, object data);
        void Start(Guid featureId);
        void Stop(Guid featureId);
    }
}
