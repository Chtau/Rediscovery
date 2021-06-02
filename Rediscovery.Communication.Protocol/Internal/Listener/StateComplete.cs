using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class StateComplete
    {
        public string IP { get; set; }
        public byte[] Raw { get; set; }

        public StateComplete(byte[] raw, string ip)
        {
            Raw = raw;
            IP = ip;
        }
    }
}
