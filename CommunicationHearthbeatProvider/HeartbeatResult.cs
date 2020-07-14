using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public class HeartbeatResult
    {
        public string DeviceId { get; set; }

        public TimeSpan? PingPongTime { get; set; }

        public DateTime? PingStartDatetimeUTC { get; set; }

        public bool OK { get; set; }

        public DateTime ResultReceived { get; set; }

        public HeartbeatResult(string deviceId, bool ok, TimeSpan? pingPongTime = null, DateTime? pingStartDatetimeUTC = null)
        {
            DeviceId = deviceId;
            OK = ok;
            PingPongTime = pingPongTime;
            PingStartDatetimeUTC = pingStartDatetimeUTC;
            ResultReceived = DateTime.UtcNow;
        }
    }
}
