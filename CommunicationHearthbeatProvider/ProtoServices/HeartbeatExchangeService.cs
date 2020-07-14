using CommunicationHeartbeatProvider;
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
        private readonly IHeartbeatStatistic _heartbeatStatistic;

        public HeartbeatExchangeService(ILoggerFactory loggerFactory, CommunicationHeartbeatProvider.IConfiguration configuration, IHeartbeatStatistic heartbeatStatistic)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatExchangeService>();
            _configuration = configuration;
            _heartbeatStatistic = heartbeatStatistic;
        }

        [Authorize(Policy = "DeviceAndConsumer")]
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
                                
                                var user = context.GetHttpContext().User;
                                string sid = user.Claims.GetSid();
                                _heartbeatStatistic.NewBeat(new CommunicationHeartbeatProvider.HeartbeatResult(sid, true, new TimeSpan((long)message.LastRoundTripTicks), new DateTime((long)message.Ticks)));
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
