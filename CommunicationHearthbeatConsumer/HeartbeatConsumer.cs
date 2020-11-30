using Rediscovery.Communication.Base;
using Grpc.Core;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Consumer.Heartbeat
{
    public class HeartbeatConsumer : IHeartbeatConsumer
    {
        public event EventHandler<RoundTripResult> ReceivedBeatRoundtrip;

        private readonly ILogger _logger;
        private ProtoHeartbeat.HeartbeatExchange.HeartbeatExchangeClient exchangeClient;
        private IClientStreamWriter<ProtoHeartbeat.PingPongMessage> _requestStream;

        private Channel channel = null;
        private CancellationTokenSource ctsBeat = null;

        public int PingResponseWaitingMilliseconds { get; set; } = 1000;

        public HeartbeatConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public bool Connect(ConsumerConnectionConfiguration connectionConfiguration)
        {
            try
            {
                channel = ChannelHelper.CreateChannel(connectionConfiguration);
                exchangeClient = new ProtoHeartbeat.HeartbeatExchange.HeartbeatExchangeClient(channel);
                return exchangeClient != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                ctsBeat?.Cancel();
                channel?.ShutdownAsync().GetAwaiter();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public void StartBeat(string identifier, string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    ctsBeat = new CancellationTokenSource();
                else
                    ctsBeat = cts;
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = exchangeClient.Beat(headers: meta, cancellationToken: ctsBeat.Token))
                    {
                        _requestStream = call.RequestStream;
                        await _requestStream.WriteAsync(new ProtoHeartbeat.PingPongMessage
                        {
                            Command = ProtoHeartbeat.PingPongMessage.Types.Command.Beat,
                            LastRoundTripTicks = 0,
                            Ticks = (ulong)DateTime.UtcNow.Ticks
                        });

                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                message.LastRoundTripTicks = (ulong)DateTime.UtcNow.Ticks - message.Ticks;

                                if (channel.State == ChannelState.Shutdown || channel.State == ChannelState.TransientFailure || channel.State == ChannelState.Idle)
                                    ReceivedBeatRoundtrip?.Invoke(this, new RoundTripResult(identifier, false));
                                else
                                    ReceivedBeatRoundtrip?.Invoke(this, new RoundTripResult(identifier, true, new TimeSpan((long)message.LastRoundTripTicks), new DateTime((long)message.Ticks)));

                                await Task.Delay(PingResponseWaitingMilliseconds);

                                message.Ticks = (ulong)DateTime.UtcNow.Ticks;
                                if (_requestStream != null)
                                {
                                    await _requestStream.WriteAsync(message);
                                }
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                            if (channel != null && (channel.State == ChannelState.Shutdown || channel.State == ChannelState.TransientFailure
                                || channel.State == ChannelState.Idle))
                                ReceivedBeatRoundtrip?.Invoke(this, new RoundTripResult(identifier, false));
                        } while (!ctsBeat.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    ReceivedBeatRoundtrip?.Invoke(this, new RoundTripResult(identifier, false));
                    _requestStream = null;
                    ctsBeat.Cancel();
                }
            });
        }
    }
}