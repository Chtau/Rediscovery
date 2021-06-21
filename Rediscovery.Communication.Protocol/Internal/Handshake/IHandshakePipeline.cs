using Rediscovery.Communication.Protocol.Internal.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Handshake
{
    internal interface IHandshakePipeline
    {
        void SynchronizeCommunication(DeviceGreetingReceived deviceGreeting, string password);
        void AcknowledgeCommunication(Action<AcknowledgeResult> acknowledgeCallback);
        void SetIdentifier(string identifier);
    }
}
