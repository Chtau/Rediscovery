using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    /// <summary>
    /// Collect UDP/TCP Packages to create our Package state object which validates and prepares the data for proxy work
    /// </summary>
    internal class PackagePartState
    {
        /// <summary>
        /// Minimum size for a valid header
        /// </summary>
        private readonly int _packageSize = 46;
        private readonly byte valueDelimiter = Encoding.UTF8.GetBytes("+").First();

        public DateTime SenderTimestamp { get; private set; } = DateTime.UtcNow;
        public int PayloadSize { get; private set; }
        public int PayloadPartSize { get; private set; }
        public string Checksum { get; private set; }
        public string SenderIdentifier { get; private set; }
        public string ReceiverIdentifier { get; private set; }
        public byte[] PayloadPart { get; private set; }
        public int Index { get; private set; } = -1;

        public PackagePartState(int packageSize,
            string senderIdentifier,
            string receiverIdentifier,
            string checksum,
            int payloadSize,
            int index)
        {
            _packageSize = packageSize;
            SenderIdentifier = senderIdentifier;
            ReceiverIdentifier = receiverIdentifier;
            Checksum = checksum;
            PayloadSize = payloadSize;
            PayloadPartSize = PayloadSize; // set maximum value because if we are smaller we pad teh number with leading zero
            Index = index;
        }

        public PackagePartState(byte[] receivedPackage)
        {
            OnParsePackage(receivedPackage);
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
            PayloadPartSize = PayloadPart.Length;
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

        /// <summary>
        /// Validates the package structur
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(SenderIdentifier)
                && !string.IsNullOrWhiteSpace(ReceiverIdentifier)
                && !string.IsNullOrWhiteSpace(Checksum)
                && PayloadSize > 0
                && Index != -1;
        }

        internal string DumpHeader()
        {
            return $"{nameof(SenderIdentifier)}:\"{SenderIdentifier}\";{nameof(ReceiverIdentifier)}:\"{ReceiverIdentifier}\";{nameof(Checksum)}:\"{Checksum}\";{nameof(PayloadSize)}:{PayloadSize};{nameof(Index)}:{Index}";
        }

        private List<byte> OnCreateSenderPackage(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                dateTime = DateTime.UtcNow;
            SenderTimestamp = dateTime.Value;
            var raw = new List<byte>(_packageSize); // Convert.FromBase64String => 12 + 12 + 6 + 7 + 12 + 3
            raw.AddRange(Convert.FromBase64String(SenderIdentifier)); // 12 byte = sender device (local)
            raw.AddRange(Convert.FromBase64String(ReceiverIdentifier)); // 12 byte = receiver device (remote)
            raw.AddRange(Convert.FromBase64String(Checksum)); // 12 byte = checksum MD5 first 16 characters (is at the same time the overall package identifier)
            raw.AddRange(Convert.FromBase64String(SenderTimestamp.ToString("mmssffff"))); // 6 byte = sender timestamp format "minutes-seconds-tousends of second"
            raw.AddRange(Encoding.UTF8.GetBytes($"+{PayloadSize}+")); // ?? byte = length of the total payload
            string formatPartPayload = $"D{PayloadSize.ToString().Length}";
            raw.AddRange(Encoding.UTF8.GetBytes($"+{PayloadPartSize.ToString(formatPartPayload)}+")); // ?? byte = can't be longer then the payload size
            raw.AddRange(Encoding.UTF8.GetBytes($"+{Index}+")); // ?? byte = package index

            return raw;
        }

        private bool OnParsePackage(byte[] raw)
        {
            if (raw?.Length > 0 && raw.Length >= 46)
            {
                var rawList = raw.ToList();
                SenderIdentifier = Convert.ToBase64String(rawList.Take(12).ToArray());
                rawList.RemoveRange(0, 12);
                ReceiverIdentifier = Convert.ToBase64String(rawList.Take(12).ToArray());
                rawList.RemoveRange(0, 12);
                Checksum = Convert.ToBase64String(rawList.Take(12).ToArray());
                rawList.RemoveRange(0, 12);
                var timestamp = Convert.ToBase64String(rawList.Take(6).ToArray());
                SenderTimestamp = DateTime.ParseExact(timestamp, "mmssffff", null);
                rawList.RemoveRange(0, 6);

                // payload size
                rawList.RemoveRange(0, 1); // remove delimiter
                var payloadSizeEndIndex = rawList.IndexOf(valueDelimiter);
                var payloadSize = Encoding.UTF8.GetString(rawList.Take(payloadSizeEndIndex).ToArray());
                PayloadSize = int.Parse(payloadSize);
                rawList.RemoveRange(0, payloadSizeEndIndex + 1);

                // payload part size
                rawList.RemoveRange(0, 1); // remove delimiter
                var payloadPartSizeEndIndex = rawList.IndexOf(valueDelimiter);
                var payloadPartSize = Encoding.UTF8.GetString(rawList.Take(payloadPartSizeEndIndex).ToArray());
                PayloadPartSize = int.Parse(payloadPartSize);
                rawList.RemoveRange(0, payloadPartSizeEndIndex + 1);

                // index
                rawList.RemoveRange(0, 1); // remove delimiter
                var indexEndIndex = rawList.IndexOf(valueDelimiter);
                var index = Encoding.UTF8.GetString(rawList.Take(indexEndIndex).ToArray());
                Index = int.Parse(index);
                rawList.RemoveRange(0, indexEndIndex + 1);

                PayloadPart = rawList.Take(PayloadPartSize).ToArray();

                return true;
            }
            return false;
        }
    }
}
