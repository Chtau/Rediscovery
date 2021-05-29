using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DataListener : BaseListener
    {

        public DataListener(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline) : base(protocolLogger, packagePipeline, nameof(DataListener))
        {

        }

        internal override void OnStateObjectComplete(byte[] data)
        {
            base.OnStateObjectComplete(data);
        }
    }
}
