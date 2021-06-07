using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    [Obsolete("Use Pipeline & Communication instead")]
    internal class LowDataSender : BaseSender
    {
        public LowDataSender(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline) : base(protocolLogger, packagePipeline)
        {

        }
    }
}
