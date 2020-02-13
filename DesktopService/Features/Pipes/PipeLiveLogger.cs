using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public class PipeLiveLogger : IPipeLiveLogger
    {
        private const string RediscoveryHub = "rediscoveryhub";
        private readonly IPCPipe.IPipeClient _pipeClient;
        private readonly ILogger<PipeLiveLogger> _logger;
        private readonly Models.PipeSettings _pipeSettings;

        public PipeLiveLogger(IPCPipe.IPipeClient pipeClient, ILoggerFactory loggerFactory,
            IOptions<Models.PipeSettings> pipeSettings)
        {
            _pipeClient = pipeClient;
            _logger = loggerFactory.CreateLogger<PipeLiveLogger>();
            _pipeSettings = pipeSettings.Value;
        }

        public void Log(LiveLoggerModel liveLoggerModel)
        {
            try
            {
                if (_pipeClient.TryConnect(RediscoveryHub))
                {
                    var logData = Newtonsoft.Json.JsonConvert.SerializeObject(liveLoggerModel);
                    _pipeClient.Send(RediscoveryHub, logData);
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
