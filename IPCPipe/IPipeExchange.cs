using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    public interface IPipeExchange
    {
        void Init(string hub, string hub_sender, string hub_receiver);
        event EventHandler<string> DataReceived;
        void Send(string data);
    }
}
