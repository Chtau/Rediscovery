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
                    //var result = OnReadStream(server);
                    StreamReader reader = new StreamReader(server);
                    StreamWriter writer = new StreamWriter(server);
                    System.Diagnostics.Debug.Print("Provider start listen");

                    var requestedResource = reader.ReadLine();//.ReadToEnd();
                    var resourceValues = resourceCallback.Invoke(requestedResource);
                    System.Diagnostics.Debug.Print("Provider send requested resource => " + requestedResource);
                    writer.Write(requestedResource + Environment.NewLine);
                    writer.Flush();
                    /*while (true)
                    {
                        var requestedResource = reader.ReadToEnd();
                        var resourceValues = resourceCallback.Invoke(requestedResource);
                        System.Diagnostics.Debug.Print("Provider send requested resource => " + requestedResource);
                        writer.Write(requestedResource);
                        writer.Flush();
                    }*/

                    //var requestedResource = OnReadStream(server);
                    //var resourceValues = resourceCallback.Invoke(requestedResource);
                    //OnWriteStream(server, resourceValues);
                    /*if (server.IsConnected)
                        server.Disconnect();*/
                    server.Dispose();
                }
            });
        }

        public void Receiver<T>(string hub, string requestedResource, Action<Models.PipeResource<T>> callback)
        {
            Task.Run(async () =>
            {
                var client = (NamedPipeClientStream)OnCreateClientHub(hub);
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                //OnWriteStream(client, requestedResource);
                writer.Write(requestedResource + Environment.NewLine);
                writer.Flush();

                bool finished = false;
                while (!finished)
                {
                    var result = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        System.Diagnostics.Debug.Print("Receiver received data => " + result);
                        finished = true;
                    }
                }

                //var result = reader.ReadToEnd();
                //var result = OnReadStream(client);
                //System.Diagnostics.Debug.Print("Receiver received data => " + result);
                return;
                //StreamReader reader = new StreamReader(client);
                //StreamWriter writer = new StreamWriter(client);
                System.Diagnostics.Debug.Print("Receiver write data => " + requestedResource);
                while (true)
                {
                    /*string input = Console.ReadLine();
                    if (String.IsNullOrEmpty(input)) break;
                    writer.WriteLine(input);
                    writer.Flush();
                    Console.WriteLine(reader.ReadLine());*/

                    writer.Write(requestedResource);
                    writer.Flush();
                    /*var resourceValues = reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(resourceValues))
                    {
                        System.Diagnostics.Debug.Print("Receiver got resource => " + requestedResource);
                    }*/
                }
            });
            
            

            //OnWriteStream(client, requestedResource);
            //var resourceValues = OnReadStream(client);
            //callback.Invoke(Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PipeResource<T>>(resourceValues));
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
