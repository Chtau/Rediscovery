using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly int _packageSize = 46;
        private readonly byte valueDelimiter = Encoding.UTF8.GetBytes("+").First();

        public DateTime SenderTimestamp { get; private set; } = DateTime.UtcNow;
        public long PayloadSize { get; private set; }
        public string Checksum { get; private set; }
        public string SenderIdentifier { get; private set; }
        public string ReceiverIdentifier { get; private set; }
        public byte[] PayloadPart { get; private set; }
        public int Index { get; private set; }

        public PackagePartState(int packageSize,
            string senderIdentifier,
            string receiverIdentifier,
            string checksum,
            long payloadSize,
            int index)
        {
            _packageSize = packageSize;
            SenderIdentifier = senderIdentifier;
            ReceiverIdentifier = receiverIdentifier;
            Checksum = checksum;
            PayloadSize = payloadSize;
            Index = index;
        }

        public PackagePartState(byte[] receivedPackage)
        {

        }

        /// <summary>
        /// Create a header for the current values to measure the length 
        /// which is required to set the correct payload
        /// </summary>
        /// <returns></returns>
        public int HeaderSizeOnly()
        {
            return OnCreateSenderPackage(null).Count;
        }

        /// <summary>
        /// Sets the <see cref="byte[]"/> payload for this index
        /// Use <see cref="HeaderSizeOnly"/> to calculate the correct payload length for this part
        /// </summary>
        /// <param name="payloadPart"></param>
        public void SetPayload(byte[] payloadPart)
        {
            PayloadPart = payloadPart;
        }

        /// <summary>
        /// Create a <see cref="byte[]"/> package which includes the header and payload
        /// </summary>
        /// <param name="dateTime">Sets a sender timestamp. Default is UTC now.</param>
        /// <returns><see cref="byte[]"/> with a maximal size of package size</returns>
        public byte[] CreateSenderPackage(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                dateTime = DateTime.UtcNow;
            var raw = OnCreateSenderPackage(dateTime);
            raw.AddRange(PayloadPart);
            return raw.ToArray();
        }

        private List<byte> OnCreateSenderPackage(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                dateTime = DateTime.UtcNow;
            var raw = new List<byte>(_packageSize); // Convert.FromBase64String => 12 + 12 + 6 + 7 + 12 + 3
            raw.AddRange(Convert.FromBase64String(SenderIdentifier)); // 12 byte = sender device (local)
            raw.AddRange(Convert.FromBase64String(ReceiverIdentifier)); // 12 byte = receiver device (remote)
            raw.AddRange(Convert.FromBase64String(Checksum)); // 12 byte = checksum MD5 first 16 characters (is at the same time the overall package identifier)
            raw.AddRange(Convert.FromBase64String(dateTime.Value.ToString("mmssffff"))); // 7 byte = sender timestamp format "minutes-seconds-tousends of second"
            raw.AddRange(Encoding.UTF8.GetBytes($"+{PayloadSize}+")); // ?? byte = length of the total payload
            raw.AddRange(Encoding.UTF8.GetBytes($"+{Index}+")); // ?? byte = package index
            return raw;
        }
    }
}
