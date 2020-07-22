using CommunicationBase;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationConsumer
{
    public interface IAuthenticationConsumerService
    {
        event EventHandler<WelcomeDeviceReply> ReceivedWelcomeReply;
        event EventHandler<SharedBase.Connection.Manifest> ReceivedManifestReply;
        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        bool Disconnect();
        void SendWelcome(WelcomeDeviceMessage message, Action<WelcomeDeviceReply> callback = null);
        void RequestManifest(string token, Action<SharedBase.Connection.Manifest> callback = null);
    }
}
