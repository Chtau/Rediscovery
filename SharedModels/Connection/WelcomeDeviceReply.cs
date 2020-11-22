using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Connection
{
    public class WelcomeDeviceReply
    {
        public Enums.ConnectionState State { get; set; }
        public string Token { get; set; }
    }
}
