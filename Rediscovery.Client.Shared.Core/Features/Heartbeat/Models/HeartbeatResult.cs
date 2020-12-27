using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Heartbeat.Models
{
    public class HeartbeatResult<T>
    {
        public T Entity { get; }

        public TimeSpan? RoundTripTime { get; }

        public DateTime? RoundTripStartDatetimeUTC { get; }

        public bool RoundTripOK { get; set; }

        public HeartbeatResult(T entity, bool roundTripOK, TimeSpan? roundTripTime = null, DateTime? roundTripStartDatetimeUTC = null)
        {
            Entity = entity;
            RoundTripOK = roundTripOK;
            RoundTripTime = roundTripTime;
            RoundTripStartDatetimeUTC = roundTripStartDatetimeUTC;
        }
    }
}
