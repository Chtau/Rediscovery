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
        private const string RediscoveryHub = "rediscoveryhublivelogger";
        private readonly IPCPipe.IPipeClient _pipeClient;
        private readonly ILogger<RemoteResourcesLiveLogger> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;

        private DateTime lastFailedConnection = DateTime.MinValue;
        private int connectionsFailed = 0;

        public RemoteResourcesLiveLogger(IPCPipe.IPipeClient pipeClient, ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings)
        {
            _pipeClient = pipeClient;
            _logger = loggerFactory.CreateLogger<RemoteResourcesLiveLogger>();
            _remoteResourceSettings = remoteResourceSettings.Value;
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
                        var logData = Newtonsoft.Json.JsonConvert.SerializeObject(liveLoggerModel);
                        _pipeClient.Send(RediscoveryHub, logData);
                    } catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"LiveLogger PipeClient failed ({connectionsFailed}) Exception:" + ex.ToString());
                        Console.ResetColor();
                        connectionsFailed++;
                        lastFailedConnection = DateTime.UtcNow;
                    }
                    /*if (_pipeClient.TryConnect(RediscoveryHub))
                    {
                        var logData = Newtonsoft.Json.JsonConvert.SerializeObject(liveLoggerModel);
                        _pipeClient.Send(RediscoveryHub, logData);
                    }
                    else
                    {
                        connectionsFailed++;
                        lastFailedConnection = DateTime.UtcNow;
                    }*/
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
