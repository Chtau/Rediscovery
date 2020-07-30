using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    public interface IPipeClient
    {
        event EventHandler<string> FailedToConnect;
        event EventHandler<string> DataReceived;
        void Listen(string hub, Action<string> callback = null);
        void Send(string hub, string data);
        void Disconnect(string hub = null);
        bool TryConnect(string hub, int timeoutMS = 500);
    }
}
