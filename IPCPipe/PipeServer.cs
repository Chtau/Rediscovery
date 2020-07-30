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

        public NamedPipeServerStream LastServerStream { get; private set; }

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
                            try
                            {
                                LastServerStream = (NamedPipeServerStream)OnCreateHub(hub);
                                System.Diagnostics.Debug.Print("Wait for Client connection");
                                LastServerStream.WaitForConnection();
                                OnReadStream(LastServerStream, callback);
                                if (LastServerStream.IsConnected)
                                    LastServerStream.Disconnect();
                            } catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Print("IPC Listen Server Loop Exception:" + ex.ToString());
                            }
                        }
                        
                    }
                    finally
                    {
                        if (hubs.ContainsKey(hub))
                            hubs.Remove(hub);
                    }
                });
            }

            /*Task.Run(() =>
            {
                while (true)
                {
                    var server = (NamedPipeServerStream)OnCreateServerHub(hub);
                    System.Diagnostics.Debug.Print("Wait for Client connection");
                    server.WaitForConnection();
                    StreamReader reader = new StreamReader(server);
                    StreamWriter writer = new StreamWriter(server);
                    System.Diagnostics.Debug.Print("Provider start listen");

                    var requestedResource = reader.ReadLine();
                    var resourceValues = resourceCallback.Invoke(requestedResource);
                    System.Diagnostics.Debug.Print("Provider send requested resource => " + requestedResource);
                    writer.Write(resourceValues + Environment.NewLine);
                    writer.Flush();
                    server.Dispose();
                }
            });*/
        }
    }
}
