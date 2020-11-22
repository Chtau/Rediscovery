using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Device
{
    public class HeartbeatActiveDeviceDetail
    {
        public string Sid { get; set; }
        public DateTime LastBeat { get; set; } = DateTime.UtcNow;
    }
}
