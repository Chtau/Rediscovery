using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class LowDataListener : BaseListener
    {
        public LowDataListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, "LowData")
        {

        }

        public override void OnDoWork()
        {

        }
    }
}
