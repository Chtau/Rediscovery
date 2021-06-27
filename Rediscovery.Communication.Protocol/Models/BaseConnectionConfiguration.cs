using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public abstract class BaseConnectionConfiguration<TListen, TSend> : IConnectionConfiguration
    {
        public BaseConnectionConfiguration(TListen listenPort, TSend sendPort, int packageSize)
        {
            ListenPort = listenPort;
            SendPort = sendPort;
            PackageSize = packageSize;
        }

        public TListen ListenPort { get; }
        public TSend SendPort { get; }
        public int PackageSize { get; }
    }
}
