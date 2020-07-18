using CommunicationBase;
using Grpc.Core;
using Heartbeat;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationHeartbeatConsumer
{
    public class HeartbeatConsumer : IHeartbeatConsumer
    {
        public event EventHandler<RoundTripResult> ReceivedBeatRoundtrip;

        private readonly ILogger _logger;
        private HeartbeatExchange.HeartbeatExchangeClient exchangeClient;
        private IClientStreamWriter<PingPongMessage> _requestStream;

        private Channel channel = null;
        private CancellationTokenSource ctsBeat = null;

        public int PingResponseWaitingMilliseconds { get; set; } = 1000;

        public HeartbeatConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public bool Connect(string ipAddress, int port, string certificatePEM)
        {
            try
            {
                var channelCredentials = new SslCredentials(certificatePEM);
                channel = new Channel(ipAddress, port, channelCredentials);
                exchangeClient = new HeartbeatExchange.HeartbeatExchangeClient(channel);
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
                        await _requestStream.WriteAsync(new PingPongMessage
                        {
                            Command = PingPongMessage.Types.Command.Beat,
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
                                await _requestStream.WriteAsync(message);
                                // TODO: only for test
                                _logger.LogTrace("New Heartbeat send");
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