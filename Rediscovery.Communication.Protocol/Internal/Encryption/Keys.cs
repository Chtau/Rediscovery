using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Keys
    {
        public string Private { get; }
        public string Public { get; }

        public Keys(string @private, string @public)
        {
            Private = @private;
            Public = @public;
        }
    }
}
