using Rediscovery.Communication.Protocol.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class Traffic
    {
        public int IncomingPackagesCompleted { get; private set; }
        public int IncomingPackageParts { get; private set; }
        public int OutgoingPackageParts { get; private set; }
        public long IncomingDataBytes { get; private set; }
        public long OutgoingDataBytes { get; private set; }

        public string IncomingDataBytesReadable() => IncomingDataBytes.Size();
        public string OutgoingDataBytesReadable() => OutgoingDataBytes.Size();

        internal void AddIncomingPackageParts() => IncomingPackageParts++;
        internal void AddOutgoingPackageParts() => OutgoingPackageParts++;
        internal void AddIncomingPackagesCompleted() => IncomingPackagesCompleted++;
        internal void ResetPackage()
        {
            IncomingPackageParts = 0;
            OutgoingPackageParts = 0;
            IncomingPackagesCompleted = 0;
        }
        internal void AddIncomingBytes(long value) => IncomingDataBytes += value;
        internal void AddOutgoingBytes(long value) => OutgoingDataBytes += value;
    }
}
 