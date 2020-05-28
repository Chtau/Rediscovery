using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceProvider
{
    public class RemoteResourcesSenderService : IRemoteResourcesSenderService
    {
        private readonly IHubContext<RemoteResourceHub> _hubContext;
        private readonly ILogger<RemoteResourcesSenderService> _logger;
        private readonly IResourcesRepository _resourcesRepository;

        public RemoteResourcesSenderService(
            IHubContext<RemoteResourceHub> hubContext,
            IResourcesRepository resourcesRepository,
            ILoggerFactory loggerFactory)
        {
            _hubContext = hubContext;
            _logger = loggerFactory.CreateLogger<RemoteResourcesSenderService>();
            _resourcesRepository = resourcesRepository;
        }

        public void AddActiveDevice(string userId)
        {
            if (!ActiveUserHandler.UserIds.Contains(userId))
                ActiveUserHandler.UserIds.Add(userId);
            SendActiveDeviceInfo();
        }

        public void RemoveActiveDevice(string userId)
        {
            if (ActiveUserHandler.UserIds.Contains(userId))
                ActiveUserHandler.UserIds.Remove(userId);
            SendActiveDeviceInfo();
        }

        public void SendActiveDeviceInfo()
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("ActiveDeviceInfo", _resourcesRepository.GetResourceActiveDeviceInfo());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendActiveDeviceInfo send remote resource");
            }
        }

        public void SendDeviceInfo()
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("DeviceInfo", _resourcesRepository.GetResourceDeviceInfo());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendDeviceInfo send remote resource");
            }
        }

        public void SendServiceFeature()
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("ServiceFeature", _resourcesRepository.GetResourceDeviceFeature());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendServiceFeature send remote resource");
            }
        }

        public void SendLoggerEntry(SharedBase.Logging.LoggerEntry liveLoggerModel)
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("LogEntry", liveLoggerModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendLoggerEntry send remote resource");
            }
        }

        public void SendPendingAuthenticationDevices()
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("PendingAuthenticationDevices", _resourcesRepository.GetResourcePendingAuthenticationDevices());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendPendingAuthenticationDevices send remote resource");
            }
        }

        public void SendFeatureDetails(Guid featureId)
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("FeatureDetailsSettings", featureId, _resourcesRepository.GetResourceDeviceFeatureSettings(featureId));
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("FeatureDetailsProfiles", featureId, _resourcesRepository.GetResourceDeviceFeatureProfiles(featureId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendFeatureDetails send remote resource");
            }
        }

        public void SendFeatureDetailsUI(Guid featureId)
        {
            try
            {
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("FeatureDetailsSettingsUI", featureId, _resourcesRepository.GetResourceDeviceFeatureSettingsUI(featureId));
                _hubContext.Clients.Group(GroupNames.Admin).SendAsync("FeatureDetailsProfilesUI", featureId, _resourcesRepository.GetResourceDeviceFeatureProfilesUI(featureId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendFeatureDetailsUI send remote resource");
            }
        }
    }
}
