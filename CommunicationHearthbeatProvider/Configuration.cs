using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Heartbeat.Provider
{
    public class Configuration : IConfiguration
    {
        public int PongResponseWaitingMilliseconds { get; set; } = 1000;
    }
}
