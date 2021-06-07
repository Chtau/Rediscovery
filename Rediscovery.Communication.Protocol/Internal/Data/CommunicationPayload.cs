using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    public class CommunicationPayload
    {
        public byte[] Payload { get; }
        public string ReceiverIdentifier { get; }

        public CommunicationPayload(byte[] payload, string receiverIdentifier)
        {
            Payload = payload;
            ReceiverIdentifier = receiverIdentifier;
        }
    }
}
