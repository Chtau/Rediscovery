using CommunicationResourceProvider;
using DALDesktopService.Models;
using DesktopService.Features.DeviceFeature;
using DesktopService.Map;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using SharedCoreModels;
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
        private readonly ILogger<RemoteResourcesRepository> _logger;

        public RemoteResourcesRepository(
            DeviceFeature.IFeatureService featureService,
            DALDesktopService.Repository.IDeviceRepository deviceRepository,
            DALDesktopService.Repository.IDevicePendingAuthenticationRepository devicePendingAuthenticationRepository,
            ILoggerFactory loggerFactory)
        {
            _featureService = featureService;
            _deviceRepository = deviceRepository;
            _devicePendingAuthenticationRepository = devicePendingAuthenticationRepository;
            _logger = loggerFactory.CreateLogger<RemoteResourcesRepository>();
        }

        public List<SharedCoreModels.DeviceFeature> GetResourceDeviceFeature()
        {
            var features = _featureService.GetFeaturesManifest();
            return (from x in features
                    select x.ToDeviceFeature()
                    ).ToList();
        }

        public List<SharedCoreModels.DeviceInfo> GetResourceDeviceInfo()
        {
            var users = _deviceRepository.GetAll().GetAwaiter().GetResult();
            return (from x in users
                    select x.ToDeviceInfo()
                    ).ToList();
        }

        public List<SharedCoreModels.DeviceInfo> GetResourceActiveDeviceInfo()
        {
            var allUsers = from x in ActiveUserHandler.UserIds select new Guid(x);
            var users = _deviceRepository.GetAll().GetAwaiter().GetResult();
            return (from x in users
                    join y in allUsers on x.Id equals y
                    select x.ToDeviceInfo()
                    ).ToList();
        }

        public void DeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo)
        {
            try
            {
                _deviceRepository.DeleteDevice(deviceInfo.Id).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteDeviceInfo error for Id:{deviceInfo?.Id} Name:{deviceInfo?.Name}");
            }
        }

        public void UpdateDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo)
        {
            try
            {
                _deviceRepository.SaveDevice(deviceInfo?.ToDevice()).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteDeviceInfo error for Id:{deviceInfo?.Id} Name:{deviceInfo?.Name}");
            }
        }

        public List<DeviceInfo> GetResourcePendingAuthenticationDevices()
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

        public List<DeviceFeatureProfil> GetResourceDeviceFeatureProfiles(Guid featureId)
        {
            return _featureService.GetFeatureProfiles(featureId);
        }

        public DeviceFeatureSetting GetResourceDeviceFeatureSettings(Guid featureId)
        {
            return _featureService.GetFeatureSettings(featureId);
        }

        public byte[] GetResourceDeviceFeatureSettingsUI(Guid featureId)
        {
            var archivePath = _featureService.GetFeatureSettingsUIArchivePath(featureId);
            if (!string.IsNullOrWhiteSpace(archivePath) && System.IO.File.Exists(archivePath))
            {
                return System.IO.File.ReadAllBytes(archivePath);
            }
            return null;
        }

        public byte[] GetResourceDeviceFeatureProfilesUI(Guid featureId)
        {
            throw new NotImplementedException();
        }

        public bool SaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting)
        {
            throw new NotImplementedException();
        }

        public bool SaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFeatureProfile(Guid featureId, string profileId)
        {
            throw new NotImplementedException();
        }
    }
}
