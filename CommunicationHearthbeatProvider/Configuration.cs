using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Heartbeat
{
    public class Configuration : IConfiguration
    {
        public int PongResponseWaitingMilliseconds { get; set; } = 1000;
    }
}
