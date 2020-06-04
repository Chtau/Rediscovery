using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationConsumer
{
    public interface IAuthenticationConsumerService
    {
        event EventHandler<SharedCoreModels.WelcomeDeviceReply> ReceivedWelcomeReply;
        event EventHandler<SharedCoreModels.Manifest> ReceivedManifestReply;
        void Connect(string ipAddress, int port, string certificatePEM);
        void SendWelcome(SharedCoreModels.WelcomeDeviceMessage message);
        void RequestManifest();
    }
}
