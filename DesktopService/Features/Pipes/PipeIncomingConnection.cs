using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public class PipeIncomingConnection : IPipeIncomingConnection
    {
        private readonly IPipe _pipe;

        public PipeIncomingConnection(IPipe pipe)
        {
            _pipe = pipe;
        }

        public async Task ShowCode(string code, string device)
        {
            // TODO: check if we find a named pipe server
            //       if there is a server we can call it and open the window
            //       if not we start the hub with the command line arguments
            //throw new NotImplementedException();
            await _pipe.SendMessage<SharedCoreModels.IncomingConnectionInfo>("rediscoveryhub", new SharedCoreModels.IncomingConnectionInfo
            {
                Code = code,
                Device = device,
                Created = DateTime.Now
            });
        }
    }
}
