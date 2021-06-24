using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class ConnectionListenConfiguration
    {
        public bool Disable { get; set; }
        public int Port { get; set; }
        public int PackageSize { get; set; }
    }
}
