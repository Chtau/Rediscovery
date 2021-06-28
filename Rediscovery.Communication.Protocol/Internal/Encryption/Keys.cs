using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Keys<T>
    {
        public T Private { get; }
        public T Public { get; }

        public Keys(T @private, T @public)
        {
            Private = @private;
            Public = @public;
        }
    }
}
