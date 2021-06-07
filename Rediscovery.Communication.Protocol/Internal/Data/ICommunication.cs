using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    public interface ICommunication
    {
        /// <summary>
        /// Received payload
        /// </summary>
        event EventHandler<CommunicationPayload> Receive;
        void Initialize(Models.BaseConfiguration configuration);
        /// <summary>
        /// Send payload
        /// </summary>
        /// <param name="communicationPayload"></param>
        /// <returns>False if the payload could not be send</returns>
        bool Send(CommunicationPayload communicationPayload);
        void Start();
        /// <summary>
        /// Stop all outstanding and active transmissions or other actions
        /// </summary>
        void Stop();
    }
}
