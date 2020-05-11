using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer
{
    public interface ILogger
    {
        void Error(Exception exception);
        void Message(string message);
    }
}
