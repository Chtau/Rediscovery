using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DiscoveryListener : BaseListener
    {
        public override int BufferSize => setting.ListenPackageBytesDiscovery;
        public override int Port => setting.ListenPortDiscovery;

        public DiscoveryListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, nameof(DiscoveryListener))
        {

        }

        internal override Socket OnGetSocket()
        {
            return Network.CreateSocket(Port, SocketType.Dgram, ProtocolType.Udp);
        }

        internal override void OnStateObjectComplete(byte[] data)
        {
            base.OnStateObjectComplete(data);
        }
    }
}
