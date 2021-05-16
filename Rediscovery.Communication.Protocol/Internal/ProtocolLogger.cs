using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class ProtocolLogger : IProtocolLogger
    {
        public void Error(Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }

        public void Information(string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
        }

        public void Warning(Exception ex)
        {
            System.Diagnostics.Trace.TraceWarning(ex.ToString());
        }

        public void Warning(string message)
        {
            System.Diagnostics.Trace.TraceWarning(message);
        }
    }
}
