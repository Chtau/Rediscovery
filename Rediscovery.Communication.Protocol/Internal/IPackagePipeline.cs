using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal interface IPackagePipeline
    {
        byte[] Outgoing<T>(T instance);
        T Incoming<T>(byte[] raw);
    }
}
