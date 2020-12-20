using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Connection
{
    public class GreetingDeviceReply
    {
        public bool UseSSL { get; set; } = false;
        public int SSLPort { get; set; }
        public string PEM { get; set; }
        public Enums.AllowConnect CanConnect { get; set; }
    }
}
