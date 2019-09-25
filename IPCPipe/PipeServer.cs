using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace IPCPipe
{
    public class PipeServer : PipeBase<NamedPipeServerStream>, IPipeServer
    {
        internal override PipeStream OnCreateHub(string hub)
        {
            var server = new NamedPipeServerStream(hub);
            //server.WaitForConnection();
            if (base.hubs.ContainsKey(hub))
                base.hubs[hub] = server;
            else
                base.hubs.Add(hub, server);
            return server;
        }

        public override void Listen(string hub, Action<string> callback = null)
        {
            if (!hubs.ContainsKey(hub))
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var server = (NamedPipeServerStream)OnCreateHub(hub);
                            System.Diagnostics.Debug.Print("Wait for Client connection");
                            server.WaitForConnection();
                            OnReadStream(server, callback);
                            if (server.IsConnected)
                                server.Disconnect();
                        }
                        
                    }
                    finally
                    {
                        if (hubs.ContainsKey(hub))
                            hubs.Remove(hub);
                    }
                });
            }
        }
    }
}
