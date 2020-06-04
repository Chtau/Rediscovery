using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationConsumer
{
    public interface IAuthenticationConsumerService
    {
        void Connect(string ipAddress, int port, string certificatePEM);
        void SendWelcome(SharedCoreModels.WelcomeDeviceMessage message);
        void RequestManifest();
    }
}
