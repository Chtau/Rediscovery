using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class Transfer
    {
        public byte[] Content { get; set; }
    }

    public enum TransportState
    {
        Unkown,
        Ok,
        Error,
        MissingPeer
    }
}
