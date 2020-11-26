using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Consumer.Logger
{
    public interface IDirectLogger
    {
        void LogException(Exception ex);
        void LogInfo(string message);
    }
}
