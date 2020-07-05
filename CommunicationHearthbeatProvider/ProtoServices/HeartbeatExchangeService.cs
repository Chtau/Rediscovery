using Grpc.Core;
using Heartbeat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHearthbeatProvider.ProtoServices
{
    public class HeartbeatExchangeService : Heartbeat.HeartbeatExchange.HeartbeatExchangeBase
    {
        private readonly ILogger<HeartbeatExchangeService> _logger;
        private readonly CommunicationHeartbeatProvider.IConfiguration _configuration;

        public HeartbeatExchangeService(ILoggerFactory loggerFactory, CommunicationHeartbeatProvider.IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatExchangeService>();
            _configuration = configuration;
        }

        [Authorize(Policy = "Device")]
        public override async Task Beat(IAsyncStreamReader<PingPongMessage> requestStream, IServerStreamWriter<PingPongMessage> responseStream, ServerCallContext context)
        {
            try
            {
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        if (message.Command == PingPongMessage.Types.Command.Beat)
                        {
                            try
                            {
                                await responseStream.WriteAsync(message);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "[Heartbeat.Beat] Pong response");
                            }
                        }
                    }
                });
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
