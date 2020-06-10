using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Connection
{
    public class WelcomeDeviceReply
    {
        public Enums.ConnectionState State { get; set; }
        public string Token { get; set; }
    }
}
