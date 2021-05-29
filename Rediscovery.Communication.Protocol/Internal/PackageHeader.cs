using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    // TODO: Package Design used for TCP
    // 1.) Fixed Length with the size of the PackageHeader Class
    // 2.) PackageHeader Instance which descripes the following data with Checksum
    // 3.) Data bytes until the size from the PackageHeader.PayloadSize is reached

    internal class PackageHeader
    {
        public DateTime SenderTimestamp { get; set; }
        public long PayloadSize { get; set; }
        public string Checksum { get; set; }
    }
}
