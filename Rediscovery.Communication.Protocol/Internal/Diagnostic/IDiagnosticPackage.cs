using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Diagnostic
{
    internal interface IDiagnosticPackage
    {
        void Send(Data.PackagePartState package);
        void Add(Data.PackagePartState package);
        void PackageComplete(string checksum);
    }
}
