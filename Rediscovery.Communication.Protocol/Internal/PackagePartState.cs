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
    internal class PackagePartState
    {
        public DateTime SenderTimestamp { get; set; }
        public long PayloadSize { get; set; }
        public string Checksum { get; set; }
        public string SenderIdentifier { get; set; }
        public string ReceiverIdentifier { get; set; }
        public byte[] PartPayload { get; set; }
        public int Index { get; set; }

        public byte[] CreateSenderPackage()
        {
            var raw = new List<byte>(46); // Convert.FromBase64String => 12 + 12 + 6 + 7 + 12 + 3
            raw.AddRange(Convert.FromBase64String(SenderIdentifier)); // 12 byte = sender device (local)
            raw.AddRange(Convert.FromBase64String(ReceiverIdentifier)); // 12 byte = receiver device (remote)
            raw.AddRange(Convert.FromBase64String(Checksum)); // 12 byte = checksum MD5 first 16 characters (is at the same time the overall package identifier)
            raw.AddRange(Convert.FromBase64String(SenderTimestamp.ToString("mmssffff"))); // 7 byte = sender timestamp format "minutes-seconds-tousends of second"
            raw.AddRange(Encoding.UTF8.GetBytes($"+{PayloadSize}+")); // ?? byte = length of the total payload
            raw.AddRange(Encoding.UTF8.GetBytes($"+{Index}+")); // ?? byte = package index

            return raw.ToArray();
        }
    }
}
