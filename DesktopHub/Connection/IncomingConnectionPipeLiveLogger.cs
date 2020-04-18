using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopHub.Connection
{
    public class IncomingConnectionPipeLiveLogger : IIncomingConnectionPipeLiveLogger
    {
        public event EventHandler<SharedCoreModels.LoggerEntryModel> LiveLoggerEntry;

        private readonly IPCPipe.IPipeServer _pipeServer;

        public IncomingConnectionPipeLiveLogger()
        {
            _pipeServer = (IPCPipe.IPipeServer)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeServer));
        }

        public void ListenForConnections()
        {
            _pipeServer.Listen("rediscoveryhublivelogger", (string data) =>
            {
                if (!string.IsNullOrWhiteSpace(data))
                {
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.LoggerEntryModel>(data);
                    LiveLoggerEntry?.Invoke(this, model);
                }
            });
        }
    }
}
