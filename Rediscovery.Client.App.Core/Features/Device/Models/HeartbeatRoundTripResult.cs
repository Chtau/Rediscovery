using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device.Models
{
    public class HeartbeatResult
    {
        public ConnectionConfiguration ConnectionConfiguration { get; }

        public TimeSpan? RoundTripTime { get; }

        public DateTime? RoundTripStartDatetimeUTC { get; }

        public bool RoundTripOK { get; set; }

        public HeartbeatResult(ConnectionConfiguration connectionConfiguration, bool roundTripOK, TimeSpan? roundTripTime = null, DateTime? roundTripStartDatetimeUTC = null)
        {
            ConnectionConfiguration = connectionConfiguration;
            RoundTripOK = roundTripOK;
            RoundTripTime = roundTripTime;
            RoundTripStartDatetimeUTC = roundTripStartDatetimeUTC;
        }
    }
}
