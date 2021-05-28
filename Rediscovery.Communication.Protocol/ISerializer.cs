using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T instance);
        T Deserialize<T>(byte[] raw);
    }
}
