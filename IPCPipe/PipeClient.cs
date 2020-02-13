using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace IPCPipe
{
    public class PipeClient : PipeBase<NamedPipeClientStream>, IPipeClient
    {
        public event EventHandler<string> FailedToConnect;

        public bool TryConnect(string hub, int timeoutMS = 500)
        {
            var client = OnCreate(hub, timeoutMS, false);
            if (client != null)
            {
                client.Dispose();
            }
            return client != null;
        }

        internal override PipeStream OnCreateHub(string hub)
        {
            var client = OnCreate(hub);
            if (client != null)
            {
                if (base.hubs.ContainsKey(hub))
                    base.hubs[hub] = client;
                else
                    base.hubs.Add(hub, client);
            }
            return client;
        }

        private PipeStream OnCreate(string hub, int timeoutMS = 500, bool raiseEvent = true)
        {
            try
            {
                var client = new NamedPipeClientStream(hub);
                client.Connect(timeoutMS);
                return client;
            }
            catch (TimeoutException)
            {
                if (raiseEvent)
                    FailedToConnect?.Invoke(this, hub);
            }
            catch (Exception)
            {
                if (raiseEvent)
                    FailedToConnect?.Invoke(this, hub);
            }
            return null;
        }
    }
}
