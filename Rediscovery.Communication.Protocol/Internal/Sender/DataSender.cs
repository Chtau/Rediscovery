using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class DataSender : BaseSender
    {
        public DataSender(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline) : base(protocolLogger, packagePipeline)
        {

        }
    }
}
