using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase
{
    public interface ILogger
    {
        void Error(Exception exception);
        void Message(string message);
    }
}
