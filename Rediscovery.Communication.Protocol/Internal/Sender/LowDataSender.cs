using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class LowDataSender : BaseSender
    {
        public LowDataSender(IProtocolLogger protocolLogger, ISerializer serializer) : base(protocolLogger, serializer)
        {

        }
    }
}
