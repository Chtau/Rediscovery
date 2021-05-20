using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class DataListener : BaseListener
    {
        public override int ListenerBufferSize => setting.ListenPackageBytesData;

        public DataListener(IProtocolLogger protocolLogger = null) : base(protocolLogger, "Data")
        {

        }

        public override void OnDoWork()
        {

        }
    }
}
