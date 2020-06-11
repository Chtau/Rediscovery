using DesktopService.Features.Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    public class RemoteResourcesLiveLogger : IRemoteResourcesLiveLogger
    {
        private readonly ILogger<RemoteResourcesLiveLogger> _logger;
        //private readonly CommunicationResourceProvider.IRemoteResourcesSenderService _remoteResourcesSenderService;

        private DateTime lastFailedConnection = DateTime.MinValue;

        public RemoteResourcesLiveLogger(ILoggerFactory loggerFactory
            //CommunicationResourceProvider.IRemoteResourcesSenderService remoteResourcesSenderService
            )
        {
            _logger = loggerFactory.CreateLogger<RemoteResourcesLiveLogger>();
            //_remoteResourcesSenderService = remoteResourcesSenderService;
        }

        public void Log(SharedBase.Logging.LoggerEntry liveLoggerModel)
        {
            try
            {
                try
                {
                    var lMsg = liveLoggerModel.Message.ToLower();
                    if (RemoteLoggerProvider.CachedLastMessage != lMsg)
                    {
                        // TODO: impl. Live logger provider consumer
                        //_remoteResourcesSenderService.SendLoggerEntry(liveLoggerModel);
                    }
                    RemoteLoggerProvider.CachedLastMessage = lMsg;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"LiveLogger failed Exception:" + ex.ToString());
                    Console.ResetColor();
                    lastFailedConnection = DateTime.UtcNow;
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
