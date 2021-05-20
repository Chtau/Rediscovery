using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class StateObjectListener
    {
        public List<byte> Data { get; set; } = new List<byte>();
        public byte[] Buffer { get; set; }
        public Socket WorkSocket { get; set; }
    }
}
