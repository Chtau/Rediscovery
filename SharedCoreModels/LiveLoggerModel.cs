using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class LiveLoggerModel
    {
        public int LogLevel { get; set; }

        public int EventId { get; set; }

        public string Message { get; set; }
    }
}
