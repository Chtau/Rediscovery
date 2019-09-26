using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    public interface IPipeResourceProvider
    {
        void Provide(string hub, Func<string, string> callback);
        void Receiver(string hub, string requestedResource, Action<string> callback);
    }
}
