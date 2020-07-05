using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatConsumer
{
    public class RoundTripResult
    {
        public TimeSpan? PingPongTime { get; set; }

        public DateTime? PingStartDatetimeUTC { get; set; }

        public bool OK { get; set; }

        public string IPAdress { get; set; }

        public int Port { get; set; }

        public RoundTripResult(string ipAdress, int port, bool ok, TimeSpan? pingPongTime = null, DateTime? pingStartDatetimeUTC = null)
        {
            IPAdress = ipAdress;
            Port = port;
            OK = ok;
            PingPongTime = pingPongTime;
            PingStartDatetimeUTC = pingStartDatetimeUTC;
        }
    }
}
