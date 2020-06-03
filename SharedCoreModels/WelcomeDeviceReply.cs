using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class WelcomeDeviceReply
    {
        public SharedCoreModels.Enums.ConnectionState State { get; set; }
        public string Token { get; set; }
    }
}
