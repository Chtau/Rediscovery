using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Logging
{
    public class LogCommandConfig
    {
        public LogCommandConfig()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public enum Command
        {
            Clear = 0,
            ChangeLogLevel = 1
        }

        public Command CommandType { get; set; }
        public string Data { get; set; }
    }
}
