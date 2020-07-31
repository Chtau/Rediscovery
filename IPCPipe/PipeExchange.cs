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
        private string hub_sender;
        private string hub_receiver;

        public PipeExchange()
        {
            _pipeClient = new IPCPipe.PipeClient();
            _pipeServer = new IPCPipe.PipeServer();
        }

        public void Init(string hub, string hub_sender, string hub_receiver)
        {
            this.hub = hub;
            this.hub_sender = hub_sender;
            this.hub_receiver = hub_receiver;
            _pipeServer.Listen($"{hub}_{this.hub_receiver}", (data) => DataReceived?.Invoke(this, data));
        }

        public void Send(string data)
        {
            _pipeClient.TryConnect($"{hub}_{hub_sender}");
            _pipeClient.Send($"{hub}_{hub_sender}", data);
        }
    }
}
