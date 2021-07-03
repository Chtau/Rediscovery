using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Keys<TPrivate, TPublic>
    {
        public TPrivate Private { get; }
        public TPublic Public { get; }

        public Keys(TPrivate @private, TPublic @public)
        {
            Private = @private;
            Public = @public;
        }
    }

    internal class Keys<T> : Keys<T, T>
    {
        public Keys(T @private, T @public) : base(@private, @public)
        {
            
        }
    }
}
