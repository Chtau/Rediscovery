using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    public interface IPipeServer
    {
        event EventHandler<object> DataReceived;
        void Listen(string hub, Action<object> callback = null);
        void Send(string hub, object data);
        void Disconnect(string hub = null);
    }
}
