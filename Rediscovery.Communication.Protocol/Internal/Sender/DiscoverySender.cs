using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class DiscoverySender : BaseSender
    {
        public override int BufferSize => setting.SendPackageBytesDiscovery;

        public DiscoverySender(IProtocolLogger protocolLogger = null) : base(protocolLogger)
        {

        }

        internal override Socket OnGetSocket(int port)
        {
            return Network.CreateSocket(port, SocketType.Dgram, ProtocolType.Udp);
        }
    }
}
