using Rediscovery.Communication.Base;
using Rediscovery.Shared.Base.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Consumer.Authentication
{
    public interface IAuthenticationConsumerService
    {
        event EventHandler<WelcomeDeviceReply> ReceivedWelcomeReply;
        event EventHandler<Rediscovery.Shared.Base.Connection.Manifest> ReceivedManifestReply;
        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        bool Disconnect();
        void SendWelcome(WelcomeDeviceMessage message, Action<WelcomeDeviceReply> callback = null);
        void RequestManifest(string token, Action<Rediscovery.Shared.Base.Connection.Manifest> callback = null);
    }
}
