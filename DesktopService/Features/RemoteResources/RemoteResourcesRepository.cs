using CommunicationResourceProvider;
using DesktopService.Features.DeviceFeature;
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
                    select new SharedCoreModels.DeviceFeature
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        MinControlIntegrationPoint = x.MinControlIntegrationPoint.ToString(),
                        MinFeatureIntegrationPoint = x.MinFeatureIntegrationPoint.ToString(),
                        Version = x.Version.ToString()
                    }).ToList();
        }

        public List<SharedCoreModels.DeviceInfo> GetResourceDeviceInfo()
        {
            var users = _deviceRepository.GetAll().GetAwaiter().GetResult();
            return (from x in users
                    select new SharedCoreModels.DeviceInfo
                    {
                        Id = x.Id,
                        AllowAccess = x.AllowAccess,
                        Name = x.DeviceName
                    }).ToList();
        }

        public List<SharedCoreModels.DeviceInfo> GetResourceActiveDeviceInfo()
        {
            var allUsers = from x in ActiveUserHandler.UserIds select new Guid(x);
            var users = _deviceRepository.GetAll().GetAwaiter().GetResult();
            return (from x in users
                    join y in allUsers on x.Id equals y
                    select new SharedCoreModels.DeviceInfo
                    {
                        Id = x.Id,
                        AllowAccess = x.AllowAccess,
                        Name = x.DeviceName
                    }).ToList();
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

        public List<DeviceInfo> GetResourcePendingAuthenticationDevices()
        {
            var devices = _devicePendingAuthenticationRepository.GetAll().GetAwaiter().GetResult();
            return (from x in devices
                    select new SharedCoreModels.DeviceInfo
                    {
                        Id = x.Id,
                        AllowAccess = false,
                        Name = x.DeviceName
                    }).ToList();
        }

        public bool ResolvePendingAuthenticationDevices(DeviceInfo deviceInfo, bool accept)
        {
            try
            {
                var pendingDevice = _devicePendingAuthenticationRepository.GetById(deviceInfo.Id).GetAwaiter().GetResult();
                if (pendingDevice != null)
                {
                    if (accept)
                    {
                        _deviceRepository.SaveDevice(new DALDesktopService.Models.Device
                        {
                            Id = Guid.NewGuid(),
                            AllowAccess = true,
                            DeviceIdentifier = pendingDevice.DeviceIdentifier,
                            DeviceName = pendingDevice.DeviceName,
                            DeviceType = pendingDevice.DeviceType,
                            Idiom = pendingDevice.Idiom,
                            Manufacturer = pendingDevice.Manufacturer,
                            Model = pendingDevice.Model,
                            OSVersion = pendingDevice.OSVersion,
                            Platform = pendingDevice.Platform
                        });
                    }
                    _devicePendingAuthenticationRepository.DeleteDevicePendingAuthentication(deviceInfo.Id).GetAwaiter().GetResult();
                    return true;
                } else
                {
                    _logger.LogCritical($"ResolvePendingAuthenticationDevices could no longer find pending authentication device (Id:{deviceInfo?.Id} Name:{deviceInfo?.Name} Accept:{accept})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ResolvePendingAuthenticationDevices error for Id:{deviceInfo?.Id} Name:{deviceInfo?.Name} Accept:{accept}");
            }
            return false;
        }
    }
}
