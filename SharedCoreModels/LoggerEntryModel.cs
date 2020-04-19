using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{

    public class LoggerEntryModel
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
        public int Type { get;private set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public string SubText { get; set; }

        public DateTime Time { get; set; }

    }
}
