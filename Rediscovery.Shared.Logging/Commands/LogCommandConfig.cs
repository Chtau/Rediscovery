using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Logging.Commands
{
    public class LogCommandConfig
    {
        public LogCommandConfig()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        
        public Command CommandType { get; set; }
        public string Data { get; set; }
    }
}
