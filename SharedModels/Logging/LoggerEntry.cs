using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Logging
{
    public class LoggerEntry
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

        private LoggerType logLevel;
        public LoggerType LogLevel
        {
            get => logLevel;
            set
            {
                logLevel = value;
                Type = (int)logLevel;
            }
        }
        public int Type { get; private set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Module { get; set; }

        public DateTime Time { get; set; }

        public string Sid { get; set; }

        public static LoggerEntry CreateEntry(string module, string message, LoggerType loggerType, string sid = null)
        {
            return new LoggerEntry
            {
                Id = Guid.NewGuid().ToString(),
                LogLevel = loggerType,
                Message = message,
                Module = module,
                Time = DateTime.Now,
                Sid = sid
            };
        }
    }
}
