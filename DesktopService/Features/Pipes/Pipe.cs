using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Pipes
{
    public class Pipe : IPipe
    {
        private readonly ILogger<Pipe> _logger;

        public Pipe(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Pipe>();
        }

        public async Task<bool> SendMessage<T>(string pipe, T message)
        {
            try
            {
                var client = new NamedPipeClientStream(pipe);
                await client.ConnectAsync(500);

                using (StreamWriter writer = new StreamWriter(client))
                {
                    writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                    //writer.WriteLine(message);
                    writer.Flush();
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
            return true;
        }
    }
}
