using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerProvider
{
    public interface IDirectLogger
    {
        void LogException(Exception ex);
        void LogInfo(string message);
    }
}
