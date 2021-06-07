using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Discovery
{
    internal interface IDiscoveryPipeline
    {
        byte[] Outgoing<T>(T instance);
        T Incoming<T>(byte[] raw);
    }
}
