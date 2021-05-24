using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal interface ISender
    {
        void Initialize(Setting setting);
        void Send(byte[] data, string ip, int port, Action<TransportState> successCallback);
    }
}
