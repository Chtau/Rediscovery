using CommunicationResourceProvider;
using DALDesktopService.Models;
using DesktopService.Features.DeviceFeature;
using DesktopService.Map;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using SharedBase.Feature;
using SharedBase.Logging;
using SharedBase.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public class RemoteResourcesRepository : IResourcesRepository
    {
        private readonly DALDesktopService.Repository.IDeviceRepository _deviceRepository;
        private readonly DALDesktopService.Repository.IDevicePendingAuthenticationRepository _devicePendingAuthenticationRepository;
        private readonly DeviceFeature.IFeatureService _featureService;
        private readonly CommunicationHeartbeatProvider.IHeartbeatStatistic _heartbeatStatistic;
        private readonly CommunicationHeartbeatProvider.IHeartbeatActive _heartbeatActive;
        private readonly CommunicationLoggerProvider.ILoggerHandler _loggerHandler;
        private readonly ILogger<RemoteResourcesRepository> _logger;

        public event EventHandler HeartbeatStatisticsChanged;
        public event EventHandler HeartbeatActiveIDsChanged;
        public event EventHandler LoggerEntiresChanged;

        public RemoteResourcesRepository(
            DeviceFeature.IFeatureService featureService,
            DALDesktopService.Repository.IDeviceRepository deviceRepository,
            DALDesktopService.Repository.IDevicePendingAuthenticationRepository devicePendingAuthenticationRepository,
            CommunicationHeartbeatProvider.IHeartbeatStatistic heartbeatStatistic,
            CommunicationHeartbeatProvider.IHeartbeatActive heartbeatActive,
            CommunicationLoggerProvider.ILoggerHandler loggerHandler,
            ILoggerFactory loggerFactory)
        {
            _featureService = featureService;
            _deviceRepository = deviceRepository;
            _devicePendingAuthenticationRepository = devicePendingAuthenticationRepository;
            _heartbeatStatistic = heartbeatStatistic;
            _heartbeatStatistic.UpdatedHeartbeatStatics += _heartbeatStatistic_UpdatedHeartbeatStatics;
            _heartbeatActive = heartbeatActive;
            _heartbeatActive.ActiveSIDsChanged += _heartbeatActive_ActiveSIDsChanged;
            _loggerHandler = loggerHandler;
            _loggerHandler.EntriesChanged += _loggerHandler_EntriesChanged;
            _logger = loggerFactory.CreateLogger<RemoteResourcesRepository>();
        }

        private void _heartbeatActive_ActiveSIDsChanged(object sender, EventArgs e)
        {
            HeartbeatActiveIDsChanged?.Invoke(this, EventArgs.Empty);
            _featureService?.ActiveDevicesChanged(_heartbeatActive?.GetActiveSIDs()?.ToArray());
        }

        private void _loggerHandler_EntriesChanged(object sender, EventArgs e)
        {
            LoggerEntiresChanged?.Invoke(this, EventArgs.Empty);
        }

        private void _heartbeatStatistic_UpdatedHeartbeatStatics(object sender, Dictionary<string, List<CommunicationHeartbeatProvider.HeartbeatResult>> e)
        {
            HeartbeatStatisticsChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<SharedBase.Device.FeatureDefinitionExtended> GetResourceDeviceFeature()
        {
            var features = _featureService.GetFeaturesManifest();
            return (from x in features
                    select x
                    ).ToList();
        }

        public List<SharedBase.Device.DeviceInfo> GetResourceDeviceInfo()
        {
            var users = _deviceRepository.GetAll().GetAwaiter().GetResult();
            return (from x in users
                    select x.ToDeviceInfo()
                    ).ToList();
        }

        [Obsolete("Active devices are now from the heartbeat")]
        public List<SharedBase.Device.DeviceInfo> GetResourceActiveDeviceInfo()
        {
            var allUsers = from x in CommunicationFeatureProvider.FeatureActiveDevices.Devices select new Guid(x);
            var users = _deviceRepository.GetAll().GetAwaiter().GetResult();
            return (from x in users
                    join y in allUsers on x.Id equals y
                    select x.ToDeviceInfo()
                    ).ToList();
        }

        public bool DeleteDeviceInfo(Guid id)
        {
            try
            {
                return _deviceRepository.DeleteDevice(id).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteDeviceInfo error for Id:{id}");
                return false;
            }
        }

        public SharedBase.Device.DeviceInfo UpdateDeviceInfo(SharedBase.Device.DeviceInfo deviceInfo)
        {
            try
            {
                var device = _deviceRepository.SaveDevice(deviceInfo?.ToDevice()).GetAwaiter().GetResult();
                return device.ToDeviceInfo();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteDeviceInfo error for Id:{deviceInfo?.Id} Name:{deviceInfo?.Name}");
                return deviceInfo;
            }
        }

        public List<SharedBase.Device.DeviceInfo> GetResourcePendingAuthenticationDevices()
        {
            var devices = _devicePendingAuthenticationRepository.GetAll().GetAwaiter().GetResult();
            return (from x in devices
                    select x.ToDeviceInfo()
                    ).ToList();
        }

        public bool ResolvePendingAuthenticationDevices(Guid deviceId, bool accept)
        {
            try
            {
                var pendingDevice = _devicePendingAuthenticationRepository.GetById(deviceId).GetAwaiter().GetResult();
                if (pendingDevice != null)
                {
                    if (accept)
                    {
                        _deviceRepository.SaveDevice(pendingDevice.ToNewDevice());
                    }
                    _devicePendingAuthenticationRepository.DeleteDevicePendingAuthentication(deviceId).GetAwaiter().GetResult();
                    return true;
                } else
                {
                    _logger.LogCritical($"ResolvePendingAuthenticationDevices could no longer find pending authentication device (Id:{deviceId} Accept:{accept})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ResolvePendingAuthenticationDevices error for Id:{deviceId} Accept:{accept}");
            }
            return false;
        }

        public List<FeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId)
        {
            return _featureService.GetFeatureProfiles(featureId);
        }

        public FeatureSetting GetResourceDeviceFeatureSettings(Guid featureId)
        {
            return _featureService.GetFeatureSettings(featureId);
        }

        public List<SharedBase.Statistics.HeartbeatStatisticItem> GetHeartbeatStatistic()
        {
            var retVal = new List<SharedBase.Statistics.HeartbeatStatisticItem>();
            try
            {
                var items = _heartbeatStatistic.Get();
                foreach (var item in items)
                {
                    var y = from x in item.Value
                            select new HeartbeatStatisticItem
                            {
                                DeviceId = x.DeviceId,
                                OK = x.OK,
                                PingPongTime = x.PingPongTime,
                                PingStartDatetimeUTC = x.PingStartDatetimeUTC,
                                ResultReceived = x.ResultReceived
                            };
                    retVal.AddRange(y);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetHeartbeatStatistic error collection statistic");
            }
            return retVal;
        }

        public List<LoggerEntry> GetLoggerEntires()
        {
            var retVal = new List<LoggerEntry>();
            try
            {
                return _loggerHandler.Get();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetLoggerEntires error collection");
            }
            return retVal;
        }

        public List<string> GetResourceActiveDeviceIds()
        {
            return _heartbeatActive.GetActiveSIDs();
        }
    }
}
