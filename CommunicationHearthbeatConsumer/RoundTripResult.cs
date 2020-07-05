using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatConsumer
{
    public class RoundTripResult
    {
        public string Identifier { get; set; }

        public TimeSpan? PingPongTime { get; set; }

        public DateTime? PingStartDatetimeUTC { get; set; }

        public bool OK { get; set; }

        public RoundTripResult(string identifier, bool ok, TimeSpan? pingPongTime = null, DateTime? pingStartDatetimeUTC = null)
        {
            Identifier = identifier;
            OK = ok;
            PingPongTime = pingPongTime;
            PingStartDatetimeUTC = pingStartDatetimeUTC;
        }
    }
}
