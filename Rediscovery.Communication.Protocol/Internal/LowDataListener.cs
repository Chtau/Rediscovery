using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class LowDataListener : BaseListener
    {
        public override int ListenerBufferSize => setting.ListenPackageBytesLowData;

        public LowDataListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, "LowData")
        {

        }

        public override void OnDoWork()
        {

        }
    }
}
