using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace IPCPipe
{
    public abstract class PipeBase<T>
    {
        public event EventHandler<object> DataReceived;

        internal Dictionary<string, PipeStream> hubs = new Dictionary<string, PipeStream>();

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
                    }
                    finally
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
                }
                else
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
                    if (item.Value is NamedPipeServerStream serverStream)
                        serverStream.Disconnect();
                    item.Value.Dispose();
                }
                hubs.Clear();
            }
            else
            {
                if (hubs.ContainsKey(hub))
                {
                    if (hubs[hub] is NamedPipeServerStream serverStream)
                        serverStream.Disconnect();
                    hubs[hub].Dispose();
                    hubs.Remove(hub);
                }
            }
        }

        internal virtual PipeStream OnCreateHub(string hub)
        {
            var server = new NamedPipeServerStream(hub);
            server.WaitForConnection();
            hubs.Add(hub, server);
            return server;
        }
    }
}
