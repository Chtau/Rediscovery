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
        void Connect(string ipAddress, int port, string certificatePEM);
        void SendWelcome(WelcomeDeviceMessage message);
        void RequestManifest(string token);
    }
}
