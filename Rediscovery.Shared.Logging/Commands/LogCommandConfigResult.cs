using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Logging.Commands
{
    public class LogCommandConfigResult
    {
        public Guid Id { get; set; }
        public bool Result { get; set; }
        public string Data { get; set; }
    }
}
