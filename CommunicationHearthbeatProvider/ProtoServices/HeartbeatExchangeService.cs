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
        public override async Task Beat(IAsyncStreamReader<PingMessage> requestStream, IServerStreamWriter<PongMessage> responseStream, ServerCallContext context)
        {
            try
            {
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        if (message.Command == PingMessage.Types.Command.Beat)
                        {
                            try
                            {
                                var offsetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);

                                var now = (ulong)(DateTime.UtcNow.Ticks - offsetDate.Ticks);
                                var ping = message.PingTime;
                                if (ping > now)
                                {
                                    _logger.LogWarning($"Heartbeat received a Ping time which is in the future. (Ping:{new DateTime((long)ping)} Pong:{new DateTime((long)now)})");
                                    ping = now;
                                }
                                ulong roundTrip = now - ping;

                                await Task.Delay(_configuration.PongResponseWaitingMilliseconds);
                                await responseStream.WriteAsync(new PongMessage
                                {
                                    Command = PongMessage.Types.Command.Beat,
                                    LastRoundTripTicks = roundTrip,
                                    PongTime = (ulong)(DateTime.UtcNow.Ticks - offsetDate.Ticks)
                                });
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
