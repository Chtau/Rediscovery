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
        internal void AddIncomingPackageParts() => IncomingPackageParts++;
        internal void AddOutgoingPackageParts() => OutgoingPackageParts++;
        internal void AddIncomingPackagesCompleted() => IncomingPackagesCompleted++;
        internal void ResetPackage()
        {
            IncomingPackageParts = 0;
            OutgoingPackageParts = 0;
            IncomingPackagesCompleted = 0;
        }
    }
}
