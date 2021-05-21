using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DataListener : BaseListener
    {
        public override int ListenerBufferSize => setting.ListenPackageBytesData;
        public override int ListenerPort => setting.ListenPortData;

        public DataListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, nameof(DataListener))
        {

        }

        internal override void OnStateObjectComplete(byte[] data)
        {
            base.OnStateObjectComplete(data);
        }
    }
}
