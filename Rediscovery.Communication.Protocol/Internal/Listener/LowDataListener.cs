using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class LowDataListener : BaseListener
    {
        public override int BufferSize => setting.ListenPackageBytesLowData;
        public override int Port => setting.ListenPortLowData;

        public LowDataListener(IProtocolLogger protocolLogger, ISerializer serializer) : base(protocolLogger, serializer, nameof(LowDataListener))
        {

        }

        internal override void OnStateObjectComplete(byte[] data)
        {
            base.OnStateObjectComplete(data);
        }
    }
}
