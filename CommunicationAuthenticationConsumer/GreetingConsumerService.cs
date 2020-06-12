using Grpc.Core;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationAuthenticationConsumer
{
    public class GreetingConsumerService : IGreetingConsumerService
    {
        private readonly ILogger _logger;

        public GreetingConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public string GreetHost(string host, string deviceIdentifier)
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    _logger.LogTrace("Consumer request Greeting");
                    var cts = new CancellationTokenSource();
                    var channel = new Channel(host, ChannelCredentials.Insecure);
                    var client = new Handshake.HandShakeExchange.HandShakeExchangeClient(channel);
                    var msg = new Handshake.GreetingMessage
                    {
                        DeviceIdentifier = deviceIdentifier
                    };
                    var reply = await client.GreetingAsync(msg, cancellationToken: cts.Token);
                    return reply.PEM;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                    return null;
                }
            });
            return task.GetAwaiter().GetResult();
        }
    }
}
