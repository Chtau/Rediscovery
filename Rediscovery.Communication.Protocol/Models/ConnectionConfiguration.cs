using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class ConnectionConfiguration
    {
        public ConnectionConfiguration(int listenPort, int sendPort, int packageSize)
        {
            ListenPort = listenPort;
            SendPort = sendPort;
            PackageSize = packageSize;
        }

        public int ListenPort { get; }
        [Obsolete("This port is device specific")]
        public int SendPort { get; }
        public int PackageSize { get; }
    }
}
