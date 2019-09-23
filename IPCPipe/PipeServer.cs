using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace IPCPipe
{
    public class PipeServer : IPipeServer
    {
        public event EventHandler<object> DataReceived;

        private Dictionary<string, NamedPipeServerStream> hubs = new Dictionary<string, NamedPipeServerStream>();

        public void Listen(string hub, Action<object> callback = null)
        {
            if (!hubs.ContainsKey(hub))
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var server = OnCreateHub(hub);
                        using (StreamReader reader = new StreamReader(server))
                        {
                            while (true)
                            {
                                var result = reader.ReadToEnd();
                                if (!string.IsNullOrWhiteSpace(result))
                                {
                                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                                    DataReceived?.Invoke(this, obj);
                                    callback?.Invoke(obj);
                                }
                            }
                        }
                    } finally
                    {
                        if (hubs.ContainsKey(hub))
                            hubs.Remove(hub);
                    }
                });
            }
        }

        public void Send(string hub, object data)
        {
            if (data != null)
            {
                if (hubs.ContainsKey(hub))
                {
                    using (StreamWriter writer = new StreamWriter(hubs[hub]))
                    {
                        writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                        writer.Flush();
                    }
                } else
                {
                    using (StreamWriter writer = new StreamWriter(OnCreateHub(hub)))
                    {
                        writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                        writer.Flush();
                    }
                }
            }
        }

        public void Disconnect(string hub = null)
        {
            if (string.IsNullOrWhiteSpace(hub))
            {
                foreach (var item in hubs)
                {
                    item.Value.Disconnect();
                    item.Value.Dispose();
                }
                hubs.Clear();
            } else
            {
                if (hubs.ContainsKey(hub))
                {
                    hubs[hub].Disconnect();
                    hubs[hub].Dispose();
                    hubs.Remove(hub);
                }
            }
        }

        private NamedPipeServerStream OnCreateHub(string hub)
        {
            var server = new NamedPipeServerStream(hub);
            server.WaitForConnection();
            hubs.Add(hub, server);
            return server;
        }
    }
}
