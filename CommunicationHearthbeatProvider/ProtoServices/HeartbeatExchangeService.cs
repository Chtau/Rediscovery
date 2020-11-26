using Rediscovery.Communication.Provider.Heartbeat;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rediscovery.Communication.Base;

namespace Rediscovery.Communication.Provider.Heartbeat.ProtoServices
{
    public class HeartbeatExchangeService : ProtoHeartbeat.HeartbeatExchange.HeartbeatExchangeBase
    {
        private readonly ILogger<HeartbeatExchangeService> _logger;
        private readonly Rediscovery.Communication.Provider.Heartbeat.IConfiguration _configuration;
        private readonly IHeartbeatStatistic _heartbeatStatistic;
        private readonly IHeartbeatActive _heartbeatActive;

        public HeartbeatExchangeService(ILoggerFactory loggerFactory, Rediscovery.Communication.Provider.Heartbeat.IConfiguration configuration, IHeartbeatStatistic heartbeatStatistic, IHeartbeatActive heartbeatActive)
        {
            _logger = loggerFactory.CreateLogger<HeartbeatExchangeService>();
            _configuration = configuration;
            _heartbeatStatistic = heartbeatStatistic;
            _heartbeatActive = heartbeatActive;
        }

        [Authorize(Policy = "DeviceAndConsumer")]
        public override async Task Beat(IAsyncStreamReader<ProtoHeartbeat.PingPongMessage> requestStream, IServerStreamWriter<ProtoHeartbeat.PingPongMessage> responseStream, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            string sid = user.Claims.GetSid();
            try
            {
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        if (message.Command == ProtoHeartbeat.PingPongMessage.Types.Command.Beat)
                        {
                            try
                            {
                                await responseStream.WriteAsync(message);
                                _heartbeatStatistic.NewBeat(new Rediscovery.Communication.Provider.Heartbeat.HeartbeatResult(sid, true, new TimeSpan((long)message.LastRoundTripTicks), new DateTime((long)message.Ticks)));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "[Heartbeat.Beat] Pong response");
                            }
                            _heartbeatActive.TryAdd(new Rediscovery.Shared.Base.Device.HeartbeatActiveDeviceDetail
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
