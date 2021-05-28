using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class DataSender : BaseSender
    {
        public override int BufferSize => setting.SendPackageBytesData;

        public DataSender(IProtocolLogger protocolLogger, ISerializer serializer) : base(protocolLogger, serializer)
        {

        }
    }
}
