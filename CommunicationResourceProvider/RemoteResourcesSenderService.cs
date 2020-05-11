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

        public void SendLoggerEntry(SharedCoreModels.LoggerEntryModel liveLoggerModel)
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
    }
}
