using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerConsumer
{
    public class DirectLogger : IDirectLogger
    {
        public void LogException(Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.ToString());
        }

        public void LogInfo(string message)
        {
            System.Diagnostics.Debug.Print(message);
        }
    }
}
