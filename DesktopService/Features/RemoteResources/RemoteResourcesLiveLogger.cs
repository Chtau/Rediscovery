using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public class RemoteResourcesLiveLogger : IRemoteResourcesLiveLogger
    {
        private readonly ILogger<RemoteResourcesLiveLogger> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly IHubContext<DesktopHubRemoteResourceHub> _hubContext;
        private readonly CommunicationResourceProvider.IRemoteResourcesSenderService _remoteResourcesSenderService;

        private DateTime lastFailedConnection = DateTime.MinValue;
        private int connectionsFailed = 0;

        public RemoteResourcesLiveLogger(IHubContext<DesktopHubRemoteResourceHub> hubContext, ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            CommunicationResourceProvider.IRemoteResourcesSenderService remoteResourcesSenderService)
        {
            _logger = loggerFactory.CreateLogger<RemoteResourcesLiveLogger>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _hubContext = hubContext;
            _remoteResourcesSenderService = remoteResourcesSenderService;
        }

        public void Log(LoggerEntryModel liveLoggerModel)
        {
            try
            {
                if (connectionsFailed > 15)
                {
                    if ((DateTime.UtcNow - lastFailedConnection).TotalMinutes > 5)
                    {
                        connectionsFailed = 0;
                        lastFailedConnection = DateTime.MinValue;
                    }
                } else
                {
                    try
                    {
                        _remoteResourcesSenderService.SendLoggerEntry(liveLoggerModel);
                        //_hubContext.Clients.Group(DesktopHubRemoteResourceHub.GroupName).SendAsync("LogEntry", liveLoggerModel);
                    } catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"LiveLogger PipeClient failed ({connectionsFailed}) Exception:" + ex.ToString());
                        Console.ResetColor();
                        connectionsFailed++;
                        lastFailedConnection = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: we should only call a local logger from here
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }
    }
}
