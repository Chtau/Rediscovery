using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class DiscoverySender : BaseSender
    {
        public override int BufferSize => setting.SendPackageBytesDiscovery;

        public DiscoverySender(IProtocolLogger protocolLogger = null) : base(protocolLogger)
        {

        }
    }
}
