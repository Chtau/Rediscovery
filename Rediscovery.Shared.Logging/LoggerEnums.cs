using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Logging
{
    public enum LoggerType
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    };

    public enum Command
    {
        State = 0,
        Clear = 1,
        ChangeLogLevel = 2
    }
}
