using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase
{
    public class Logger : ILogger
    {
        public void Error(Exception exception)
        {
            System.Diagnostics.Debug.Print(exception.ToString());
        }

        public void Message(string message)
        {
            System.Diagnostics.Debug.Print(message);
        }
    }
}
