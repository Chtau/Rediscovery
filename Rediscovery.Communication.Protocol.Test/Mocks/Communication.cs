using Rediscovery.Communication.Protocol.Internal.Data;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Test.Mocks
{
    public class Communication : Internal.Data.ICommunication
    {
        public event EventHandler<byte[]> Receive;

        public void Initialize(ConnectionConfiguration configuration)
        {
            
        }

        public bool Send(CommunicationPayload communicationPayload)
        {
            Receive?.Invoke(this, communicationPayload.Payload);
            return true;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }
}
