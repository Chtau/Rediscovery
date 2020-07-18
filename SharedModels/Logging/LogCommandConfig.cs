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
            State = 0,
            Clear = 1,
            ChangeLogLevel = 2
        }

        public Command CommandType { get; set; }
        public string Data { get; set; }
    }
}
