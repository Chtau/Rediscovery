using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Heartbeat.Provider
{
    public class HeartbeatStatistic : IHeartbeatStatistic
    {
        private readonly ILogger<HeartbeatStatistic> _logger;
        private Dictionary<string, List<HeartbeatResult>> data = new Dictionary<string, List<HeartbeatResult>>();
        private DateTime lastUpdatedStaticEvent = DateTime.UtcNow.AddMinutes(-1);
        private DateTime lastClearStatic = DateTime.UtcNow;

        public event EventHandler<Dictionary<string, List<HeartbeatResult>>> UpdatedHeartbeatStatics;

        public HeartbeatStatistic(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatStatistic>();
        }

        public void NewBeat(HeartbeatResult heartbeatResult)
        {
            try
            {
                if (data.ContainsKey(heartbeatResult.DeviceId))
                    data[heartbeatResult.DeviceId].Add(heartbeatResult);
                else
                    data.Add(heartbeatResult.DeviceId, new List<HeartbeatResult>() { heartbeatResult });

                if (lastUpdatedStaticEvent.AddSeconds(10) < DateTime.UtcNow)
                {
                    lastUpdatedStaticEvent = DateTime.UtcNow;
                    UpdatedHeartbeatStatics?.Invoke(this, data);
                }
                if (lastClearStatic.AddMinutes(5) < DateTime.UtcNow)
                {
                    lastClearStatic = DateTime.UtcNow;
                    data.Clear();
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public Dictionary<string, List<HeartbeatResult>> Get()
        {
            return data;
        }
    }
}
