using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Heartbeat
{
    public interface IConfiguration
    {
        int PongResponseWaitingMilliseconds { get; set; }
    }
}
