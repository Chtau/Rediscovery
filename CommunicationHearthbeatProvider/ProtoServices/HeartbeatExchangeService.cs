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
        private readonly IHeartbeatActive _heartbeatActive;

        public HeartbeatExchangeService(ILoggerFactory loggerFactory, CommunicationHeartbeatProvider.IConfiguration configuration, IHeartbeatStatistic heartbeatStatistic, IHeartbeatActive heartbeatActive)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatExchangeService>();
            _configuration = configuration;
            _heartbeatStatistic = heartbeatStatistic;
            _heartbeatActive = heartbeatActive;
        }

        [Authorize(Policy = "DeviceAndConsumer")]
        public override async Task Beat(IAsyncStreamReader<PingPongMessage> requestStream, IServerStreamWriter<PingPongMessage> responseStream, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            string sid = user.Claims.GetSid();
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
                                _heartbeatStatistic.NewBeat(new CommunicationHeartbeatProvider.HeartbeatResult(sid, true, new TimeSpan((long)message.LastRoundTripTicks), new DateTime((long)message.Ticks)));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "[Heartbeat.Beat] Pong response");
                            }
                            _heartbeatActive.TryAdd(new SharedBase.Device.HeartbeatActiveDeviceDetail
                            {
                                Sid = sid
                            });
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
            } finally
            {
                _heartbeatActive.TryRemove(sid);
            }
        }
    }
}
