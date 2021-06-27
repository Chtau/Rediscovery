using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class ConnectionConfigurationDiscovery : BaseConnectionConfiguration<List<int>, List<int>>
    {
        public ConnectionConfigurationDiscovery(List<int> listenPort, List<int> sendPort, int packageSize) : base(listenPort, sendPort, packageSize)
        {
            
        }

        public ConnectionConfigurationDiscovery(int listenPort, int sendPort, int packageSize) : base(new List<int> { listenPort }, new List<int> { sendPort }, packageSize)
        {

        }
    }
}
