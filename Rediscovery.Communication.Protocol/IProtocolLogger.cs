using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface IProtocolLogger
    {
        void Error(Exception ex);
        void Warning(Exception ex);
        void Warning(string message);
        void Information(string message);
        void Trace(string message);
    }
}
