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
        public event EventHandler<string> DataReceived;

        internal Dictionary<string, PipeStream> hubs = new Dictionary<string, PipeStream>();

        public virtual void Listen(string hub, Action<string> callback = null)
        {
            if (!hubs.ContainsKey(hub))
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var server = OnCreateHub(hub);
                        OnReadStream(server, callback);
                    }
                    finally
                    {
                        if (hubs.ContainsKey(hub))
                            hubs.Remove(hub);
                    }
                });
            }
        }

        internal void OnReadStream(PipeStream stream, Action<string> callback = null)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                bool active = true;
                while (active)
                {
                    var result = reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        System.Diagnostics.Debug.Print("ReadStream received data =>" + result);
                        DataReceived?.Invoke(this, result);
                        callback?.Invoke(result);
                    } else
                    {
                        active = false;
                    }
                }
            }
        }

        public void Send(string hub, string data)
        {
            if (data != null)
            {
                using (StreamWriter writer = new StreamWriter(OnCreateHub(hub)))
                {
                    writer.Write(data);
                    writer.Flush();
                }
                /*if (hubs.ContainsKey(hub))
                {
                    using (StreamWriter writer = new StreamWriter(hubs[hub]))
                    {
                        writer.Write(data);
                        writer.Flush();
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(OnCreateHub(hub)))
                    {
                        writer.Write(data);
                        writer.Flush();
                    }
                }*/
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
