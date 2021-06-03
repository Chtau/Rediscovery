using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    // TODO: Package Design used for TCP
    // TODO: Should we change to Byte flags instead of structur to reduce bytes used?
    // 1.) Fixed Length with the size of the PackageHeader Class
    // 2.) PackageHeader Instance which descripes the following data with Checksum
    // 3.) Data bytes until the size from the PackageHeader.PayloadSize is reached
    // 4.) TODO: peer receiver / hop target

    /// <summary>
    /// Collect UDP/TCP Packages to create our Package state object which validates and prepares the data for proxy work
    /// </summary>
    internal class PackageState
    {
        public DateTime SenderTimestamp { get; set; }
        public long PayloadSize { get; set; }
        public string Checksum { get; set; }
        public string SenderIdentifier { get; set; }
        public string ReceiverIdentifier { get; set; }
        public List<int> ReceivedPackageIndexes { get; set; } = new List<int>();
        public byte[] Payload { get; set; }
    }
}
