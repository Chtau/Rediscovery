using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public class Configuration : IConfiguration
    {
        public int PongResponseWaitingMilliseconds { get; set; } = 1000;
    }
}
