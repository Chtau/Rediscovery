using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class LowDataListener : BaseListener
    {
        public override int ListenerBufferSize => setting.ListenPackageBytesLowData;
        public override int ListenerPort => setting.ListenPortLowData;

        public LowDataListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, nameof(LowDataListener))
        {

        }

        internal override void OnStateObjectComplete(byte[] data)
        {
            base.OnStateObjectComplete(data);
        }
    }
}
