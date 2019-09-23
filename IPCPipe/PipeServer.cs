using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace IPCPipe
{
    public class PipeServer : PipeBase<NamedPipeServerStream>, IPipeServer
    {
        internal override PipeStream OnCreateHub(string hub)
        {
            var server = new NamedPipeServerStream(hub);
            server.WaitForConnection();
            base.hubs.Add(hub, server);
            return server;
        }
    }
}
