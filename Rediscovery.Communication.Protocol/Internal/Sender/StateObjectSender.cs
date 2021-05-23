using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal class StateObjectSender
    {
        public Socket Sender { get; set; }
        public Action<TransportState> SuccessCallback { get; set; }
    }
}
