using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class Timing
    {
        public string DeviceIdentifer { get; }
        public List<TimeSpan> Times { get; private set; } = new List<TimeSpan>();

        internal Timing(string identifer, TimeSpan time)
        {
            DeviceIdentifer = identifer;
            Times.Add(time);
        }

        internal void Add(TimeSpan time) => Times.Add(time);
    }
}
