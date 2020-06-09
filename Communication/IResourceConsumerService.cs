using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer
{
    public interface IResourceConsumerService
    {
        void Connect(string ipAddress, int port, string certificatePEM);
    }
}
