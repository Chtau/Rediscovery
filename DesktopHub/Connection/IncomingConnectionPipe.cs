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

        private readonly Pipes.IPipe _pipe;

        public IncomingConnectionPipe()
        {
            _pipe = (Pipes.IPipe)Program.ServiceProvider.GetService(typeof(Pipes.IPipe));
        }

        public void ListenForConnections()
        {
            Task.Factory.StartNew(() =>
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

                /*StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    writer.WriteLine(String.Join("", line.Reverse()));
                    writer.Flush();
                }*/
            });
        }
    }
}
