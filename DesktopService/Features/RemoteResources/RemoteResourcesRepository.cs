using CommunicationResourceProvider;
using DesktopService.Features.DeviceFeature;
using DesktopService.Features.Identity.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
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
        private readonly DAL.IDBContext _dBContext;
        private readonly DeviceFeature.IFeatureService _featureService;
        private readonly ILogger<RemoteResourcesRepository> _logger;

        public RemoteResourcesRepository(DAL.IDBContext dBContext,
            DeviceFeature.IFeatureService featureService,
            ILoggerFactory loggerFactory)
        {
            _dBContext = dBContext;
            _featureService = featureService;
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
            var users = _dBContext.Instance.Table<Device>().ToListAsync().GetAwaiter().GetResult();
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
            var users = _dBContext.Instance.Table<Device>().ToListAsync().GetAwaiter().GetResult();
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
                _dBContext.Instance.Table<Device>().DeleteAsync(x => x.Id == deviceInfo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteDeviceInfo error for Id:{deviceInfo?.Id} Name:{deviceInfo?.Name}");
            }
        }
    }
}
