using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface ILogger
    {
        void Error(Exception exception);
        void Message(string message);
    }
}
