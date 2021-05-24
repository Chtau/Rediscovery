using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal static class Identifier
    {
        public static string Create()
        {
            return $"{DateTime.Now.Ticks}@{Guid.NewGuid():N}@{Environment.MachineName}".GetHashString();
        }
    }
}
