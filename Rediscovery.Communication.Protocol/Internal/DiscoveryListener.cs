using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class DiscoveryListener : BaseListener
    {
        public override int ListenerBufferSize => setting.ListenPackageBytesDiscovery;
        public override int ListenerPort => setting.ListenPortDiscovery;

        public DiscoveryListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, "Discovery")
        {

        }

        public override void OnDoWork()
        {
            
        }
    }
}
