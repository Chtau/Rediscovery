using Microsoft.Extensions.Logging;
using Rediscovery.Shared.Base.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Provider.Heartbeat
{
    public class HeartbeatActive : IHeartbeatActive
    {
        private readonly ILogger<HeartbeatActive> _logger;
        private readonly Dictionary<string, Rediscovery.Shared.Base.Device.HeartbeatActiveDeviceDetail> _activeDeviceDetails = new Dictionary<string, Rediscovery.Shared.Base.Device.HeartbeatActiveDeviceDetail>();
        private DateTime lastChangedEvent = DateTime.UtcNow.AddMinutes(-1);
        private DateTime lastRemoveChangedEvent = DateTime.UtcNow.AddMinutes(-1);

        public event EventHandler ActiveSIDsChanged;

        public HeartbeatActive(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatActive>();
            OnTimeoutWatcher();
        }

        private void OnTimeoutWatcher()
        {
            try
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        var timeoutDateTime = DateTime.UtcNow.AddMinutes(-1);
                        var timeoutIds = (from x in _activeDeviceDetails
                                          where x.Value.LastBeat < timeoutDateTime
                                          select x.Key.ToLower())?.ToList();
                        if (timeoutIds?.Count > 0)
                        {
                            foreach (var id in timeoutIds)
                            {
                                if (_activeDeviceDetails.ContainsKey(id))
                                {
                                    _activeDeviceDetails.Remove(id);
                                }
                            }
                            ActiveSIDsChanged?.Invoke(this, EventArgs.Empty);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public HeartbeatActiveDeviceDetail Detail(string sid)
        {
            string id = sid.ToLower();
            if (_activeDeviceDetails.ContainsKey(id))
                return _activeDeviceDetails[id];
            return null;
        }

        public List<string> GetActiveSIDs()
        {
            return (from x in _activeDeviceDetails
                    select x.Value.Sid)?.ToList();
        }

        public void TryAdd(HeartbeatActiveDeviceDetail deviceDetail)
        {
            try
            {
                if (deviceDetail == null || string.IsNullOrWhiteSpace(deviceDetail.Sid))
                    return;
                string id = deviceDetail.Sid.ToLower();
                if (_activeDeviceDetails.ContainsKey(id))
                {
                    _activeDeviceDetails[id].LastBeat = deviceDetail.LastBeat;
                }
                else
                {
                    _activeDeviceDetails.Add(id, deviceDetail);
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

        public void TryRemove(string sid)
        {
            try
            {
                string id = sid.ToLower();
                if (_activeDeviceDetails.ContainsKey(id))
                {
                    _activeDeviceDetails.Remove(id);

                    if (lastRemoveChangedEvent.AddSeconds(10) < DateTime.UtcNow)
                    {
                        lastRemoveChangedEvent = DateTime.UtcNow;
                        ActiveSIDsChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}