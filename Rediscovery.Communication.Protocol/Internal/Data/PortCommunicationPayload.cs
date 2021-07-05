using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    public class PortCommunicationPayload : CommunicationPayload
    {
        public int Port { get; }
        public int PackageSize { get; }

        public PortCommunicationPayload(byte[] payload, string receiverIdentifier, int port, int packageSize) : base(payload, receiverIdentifier)
        {
            Port = port;
            PackageSize = packageSize;
        }
    }
}
