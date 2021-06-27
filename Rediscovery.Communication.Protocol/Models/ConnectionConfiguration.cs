using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class ConnectionConfiguration : BaseConnectionConfiguration<int, int>
    {
        public ConnectionConfiguration(int listenPort, int sendPort, int packageSize) : base(listenPort, sendPort, packageSize)
        {
            
        }
    }
}
