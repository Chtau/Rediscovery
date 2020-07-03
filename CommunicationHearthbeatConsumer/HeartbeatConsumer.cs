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
        public event EventHandler<TimeSpan> ReceivedBeatRoundtrip;

        private readonly ILogger _logger;
        private HeartbeatExchange.HeartbeatExchangeClient exchangeClient;
        private IClientStreamWriter<PingMessage> _requestStream;

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

        public void StartBeat(string token, CancellationTokenSource cts = null)
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
                        var offsetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                        await _requestStream.WriteAsync(new PingMessage
                        {
                            Command = PingMessage.Types.Command.Beat,
                            LastRoundTripTicks = 0,
                            PingTime = (ulong)(DateTime.UtcNow.Ticks - offsetDate.Ticks)
                        });

                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                ReceivedBeatRoundtrip?.Invoke(this, new TimeSpan((long)message.LastRoundTripTicks));

                                var offsetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                                var now = (ulong)(DateTime.UtcNow.Ticks - offsetDate.Ticks);
                                var pong = message.PongTime;
                                if (pong > now)
                                {
                                    _logger.LogWarning($"Heartbeat received a Pong time which is in the future. (Ping:{new DateTime((long)now)} Pong:{new DateTime((long)pong)})");
                                    pong = now;
                                }
                                ulong roundTrip = now - pong;

                                await Task.Delay(PingResponseWaitingMilliseconds);
                                
                                await _requestStream.WriteAsync(new PingMessage
                                {
                                    Command = PingMessage.Types.Command.Beat,
                                    LastRoundTripTicks = roundTrip,
                                    PingTime = (ulong)(DateTime.UtcNow.Ticks - offsetDate.Ticks)
                        });
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                        } while (!ctsBeat.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    _requestStream = null;
                    ctsBeat.Cancel();
                }
            });
        }
    }
}
 