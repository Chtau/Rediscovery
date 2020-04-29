using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    [Obsolete("Replace with SignalR")]
    public interface IPipeResourceProvider
    {
        void Provide(string hub, Func<string, string> resourceCallback);
        void Receiver<T>(string hub, string requestedResource, Action<Models.PipeResource<T>> callback);
    }
}
