using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace DesktopHub.Connection
{
    public class IncomingConnectionPipe : IIncomingConnectionPipe
    {
        public event EventHandler<SharedCoreModels.IncomingConnectionInfo> NewConnectionInfo;

        private readonly IPCPipe.IPipeServer _pipeServer;

        public IncomingConnectionPipe()
        {
            _pipeServer = (IPCPipe.IPipeServer)Program.ServiceProvider.GetService(typeof(IPCPipe.IPipeServer));
        }

        public void ListenForConnections()
        {
            _pipeServer.Listen("rediscoveryhub", (string data) =>
            {
                if (!string.IsNullOrWhiteSpace(data))
                {
                    var infoData = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.IncomingConnectionInfo>(data);
                    NewConnectionInfo?.Invoke(this, infoData);
                }
            });
            /*Task.Factory.StartNew(() =>
            {
                
                var server = new NamedPipeServerStream("rediscoveryhub");
                server.WaitForConnection();

                using (StreamReader reader = new StreamReader(server))
                {
                    while (true)
                    {
                        var result = reader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            NewConnectionInfo?.Invoke(this, Newtonsoft.Json.JsonConvert.DeserializeObject<SharedCoreModels.IncomingConnectionInfo>(result));
                        }
                    }
                }
            });*/
        }
    }
}
