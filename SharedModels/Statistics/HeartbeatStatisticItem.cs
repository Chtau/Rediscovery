using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Statistics
{
    public class HeartbeatStatisticItem
    {
        public string DeviceId { get; set; }

        public TimeSpan? PingPongTime { get; set; }

        public DateTime? PingStartDatetimeUTC { get; set; }

        public bool OK { get; set; }

        public DateTime ResultReceived { get; set; }
    }
}
