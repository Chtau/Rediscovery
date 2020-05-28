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
        private readonly CommunicationResourceProvider.IRemoteResourcesSenderService _remoteResourcesSenderService;

        private DateTime lastFailedConnection = DateTime.MinValue;
        private int connectionsFailed = 0;

        public RemoteResourcesLiveLogger(ILoggerFactory loggerFactory,
            CommunicationResourceProvider.IRemoteResourcesSenderService remoteResourcesSenderService)
        {
            _logger = loggerFactory.CreateLogger<RemoteResourcesLiveLogger>();
            _remoteResourcesSenderService = remoteResourcesSenderService;
        }

        public void Log(SharedBase.Logging.LoggerEntry liveLoggerModel)
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
