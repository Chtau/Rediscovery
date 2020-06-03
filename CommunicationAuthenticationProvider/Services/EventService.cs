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

        public void InvokeSendWelcomeDeviceReply(WelcomeDeviceReply welcomeDeviceReply)
        {
            SendWelcomeDeviceReply?.Invoke(this, welcomeDeviceReply);
        }
    }
}
