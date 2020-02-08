using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public class CommandQueue<I, O>
    {
        public string DeviceId { get; set; }
        public I IncomingData { get; set; }
        public O OutgoingData { get; set; }
        public DateTime Received { get; set; }
        public DateTime Send { get; set; }
        public List<Exception> Exception { get; set; }

        public CommandQueue()
        {
            Received = DateTime.UtcNow;
            Exception = new List<Exception>();
        }

        public CommandQueue(string deviceId, I incomingData): this()
        {
            DeviceId = deviceId;
            IncomingData = incomingData;
        }
    }
}
