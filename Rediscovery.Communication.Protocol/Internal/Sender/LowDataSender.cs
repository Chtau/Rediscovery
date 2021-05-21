using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class LowDataSender : BaseSender
    {
        public override int BufferSize => setting.SendPackageBytesLowData;

        public LowDataSender(IProtocolLogger protocolLogger = null) : base(protocolLogger)
        {

        }
    }
}
