using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly IPCPipe.IPipeServer _pipeServer;
        public event EventHandler<LiveLoggerModel> LoggerDataReceived;

        public LoggerService(IPCPipe.IPipeServer pipeServer)
        {
            _pipeServer = pipeServer;
        }

        public void Init()
        {
            _pipeServer.Listen("rediscoveryhublivelogger", data =>
            {
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<LiveLoggerModel>(data);
                LoggerDataReceived?.Invoke(this, model);
            });
        }
    }
}
