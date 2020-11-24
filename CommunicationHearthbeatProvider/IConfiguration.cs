using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Heartbeat.Provider
{
    public interface IConfiguration
    {
        int PongResponseWaitingMilliseconds { get; set; }
    }
}
