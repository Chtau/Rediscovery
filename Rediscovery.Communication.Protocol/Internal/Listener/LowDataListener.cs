using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class LowDataListener : BaseListener
    {
        public LowDataListener(IProtocolLogger protocolLogger, ISerializer serializer) : base(protocolLogger, serializer, nameof(LowDataListener))
        {

        }

        internal override void OnStateObjectComplete(byte[] data)
        {
            base.OnStateObjectComplete(data);
        }
    }
}
