using SharedBase.Device;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationAuthenticationProvider.Services
{
    public class EventService : IEventService
    {
        public event EventHandler<SharedCoreModels.WelcomeDeviceMessage> ReceivedWelcomeDeviceMessage;
        public event EventHandler<WelcomeDeviceReply> SendWelcomeDeviceReply;
        public event EventHandler<SharedCoreModels.Manifest> SendManifest;

        public void InvokeReceivedWelcomeDeviceMessage(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            ReceivedWelcomeDeviceMessage?.Invoke(this, welcomeDeviceMessage);
        }

        public void InvokeSendManifest(SharedCoreModels.Manifest manifest)
        {
            SendManifest?.Invoke(this, manifest);
        }

        public void InvokeSendWelcomeDeviceReply(WelcomeDeviceReply welcomeDeviceReply)
        {
            SendWelcomeDeviceReply?.Invoke(this, welcomeDeviceReply);
        }
    }
}
