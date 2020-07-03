using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public interface IConfiguration
    {
        int PongResponseWaitingMilliseconds { get; set; }
    }
}
