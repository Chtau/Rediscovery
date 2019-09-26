using IPCPipe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace IPCPipe
{
    public class PipeResourceProvider : IPipeResourceProvider
    {
        internal PipeStream OnCreateServerHub(string hub)
        {
            var server = new NamedPipeServerStream(hub);
            return server;
        }

        public void Provide(string hub, Func<string, string> resourceCallback)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var server = (NamedPipeServerStream)OnCreateServerHub(hub);
                    System.Diagnostics.Debug.Print("Wait for Client connection");
                    server.WaitForConnection();
                    var requestedResource = OnReadStream(server);
                    var resourceValues = resourceCallback.Invoke(requestedResource);
                    OnWriteStream(server, resourceValues);
                    if (server.IsConnected)
                        server.Disconnect();
                }
            });
        }

        public void Receiver<T>(string hub, string requestedResource, Action<Models.PipeResource<T>> callback)
        {
            var client = (NamedPipeClientStream)OnCreateClientHub(hub);
            OnWriteStream(client, requestedResource);
            var resourceValues = OnReadStream(client);
            callback.Invoke(Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PipeResource<T>>(resourceValues));
        }

        internal string OnReadStream(PipeStream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                System.Diagnostics.Debug.Print("ReadStream received data =>" + result);
                return result;
            }
        }

        internal void OnWriteStream(PipeStream stream, string data)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
                writer.Flush();
            }
        }

        internal PipeStream OnCreateClientHub(string hub)
        {
            var client = new NamedPipeClientStream(hub);
            client.Connect();
            return client;
        }
    }
}
