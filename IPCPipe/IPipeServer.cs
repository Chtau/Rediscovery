using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    [Obsolete("Replace with SignalR")]
    public interface IPipeServer
    {
        event EventHandler<string> DataReceived;
        void Listen(string hub, Action<string> callback = null);
        void Send(string hub, string data);
        void Disconnect(string hub = null);
    }
}
