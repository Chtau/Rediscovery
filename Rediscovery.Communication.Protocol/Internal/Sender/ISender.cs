using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Sender
{
    internal interface ISender
    {
        void Initialize(Models.BaseConfiguration configuration);
        void Send(byte[] data, string ip, int port, Action<TransportState> successCallback);
        /// <summary>
        /// Stop all outstanding and active transmissions or other actions
        /// </summary>
        void Stop();
    }
}
