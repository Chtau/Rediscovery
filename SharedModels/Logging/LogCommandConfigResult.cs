using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Logging
{
    public class LogCommandConfigResult
    {
        public Guid Id { get; set; }
        public bool Result { get; set; }
        public string Data { get; set; }
    }
}
