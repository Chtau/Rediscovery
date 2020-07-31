using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe
{
    public class PipeExchange : IPipeExchange
    {
        public event EventHandler<string> DataReceived;

        private readonly IPipeClient _pipeClient;
        private readonly IPipeServer _pipeServer;

        private string hub;

        public PipeExchange()
        {
            _pipeClient = new IPCPipe.PipeClient();
            _pipeServer = new IPCPipe.PipeServer();
        }

        public void Init(string hub)
        {
            this.hub = hub;
        }

        public void Send(string data)
        {
            throw new NotImplementedException();
        }
    }
}
