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
            Task.Run(() =>
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
            });
        }

        public void Receiver<T>(string hub, string requestedResource, Action<Models.PipeResource<T>> callback)
        {
            Task.Run(() =>
            {
                var client = (NamedPipeClientStream)OnCreateClientHub(hub);
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                writer.Write(requestedResource + Environment.NewLine);
                writer.Flush();

                bool finished = false;
                while (!finished)
                {
                    var resourceValues = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(resourceValues))
                    {
                        System.Diagnostics.Debug.Print("Receiver received data => " + resourceValues);
                        callback.Invoke(Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PipeResource<T>>(resourceValues));
                        finished = true;
                    }
                }
                return;
            });
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
