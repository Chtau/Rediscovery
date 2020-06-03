using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider.Services
{
    public interface IEventService
    {
        event EventHandler<SharedCoreModels.WelcomeDeviceMessage> ReceivedWelcomeDeviceMessage;
        event EventHandler<SharedCoreModels.WelcomeDeviceReply> SendWelcomeDeviceReply;
        event EventHandler<SharedCoreModels.Manifest> SendManifest;
        void InvokeSendWelcomeDeviceReply(SharedCoreModels.WelcomeDeviceReply welcomeDeviceReply);
        void InvokeReceivedWelcomeDeviceMessage(SharedCoreModels.WelcomeDeviceMessage welcomeDeviceMessage);
        void InvokeSendManifest(SharedCoreModels.Manifest manifest);
    }
}
