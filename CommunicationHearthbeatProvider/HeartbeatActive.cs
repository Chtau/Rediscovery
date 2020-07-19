using Microsoft.Extensions.Logging;
using SharedBase.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public class HeartbeatActive : IHeartbeatActive
    {
        private readonly ILogger<HeartbeatActive> _logger;
        private Dictionary<string, SharedBase.Device.HeartbeatActiveDeviceDetail> activeDeviceDetails = new Dictionary<string, SharedBase.Device.HeartbeatActiveDeviceDetail>();
        private DateTime lastChangedEvent = DateTime.UtcNow.AddMinutes(-1);

        public event EventHandler ActiveSIDsChanged;

        public HeartbeatActive(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatActive>();
        }

        public HeartbeatActiveDeviceDetail Detail(string sid)
        {
            string id = sid.ToLower();
            if (activeDeviceDetails.ContainsKey(id))
                return activeDeviceDetails[id];
            return null;
        }

        public List<string> GetActiveSIDs()
        {
            return (from x in activeDeviceDetails
                   select x.Value.Sid)?.ToList();
        }

        public void TryAdd(HeartbeatActiveDeviceDetail deviceDetail)
        {
            try
            {
                if (deviceDetail == null || string.IsNullOrWhiteSpace(deviceDetail.Sid))
                    return;
                string id = deviceDetail.Sid.ToLower();
                if (activeDeviceDetails.ContainsKey(id))
                {
                    // update device detail
                } else
                {
                    activeDeviceDetails.Add(id, deviceDetail);
                }

                if (lastChangedEvent.AddSeconds(10) < DateTime.UtcNow)
                {
                    lastChangedEvent = DateTime.UtcNow;
                    ActiveSIDsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
