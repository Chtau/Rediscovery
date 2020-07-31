using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    public interface IPipeExchange
    {
        void Init(string hub);
        event EventHandler<string> DataReceived;
        void Send(string data);
    }
}
