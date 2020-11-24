using Grpc.Core;
using Rediscovery.Shared.Base.Connection;
using Rediscovery.Shared.Base.Extensions;
using Rediscovery.Shared.Base.Logging;
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

        private Channel channel = null;
        private CancellationTokenSource cts = null;

        public GreetingConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public bool Disconnect()
        {
            try
            {
                cts?.Cancel();
                channel?.ShutdownAsync().GetAwaiter();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public GreetingDeviceReply GreetHost(string host, int port, GreetingDeviceMessage greetingDevice, int secondsTimeout = 2)
        {
            cts = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                try
                {
                    _logger.LogTrace("Consumer request Greeting");
                    channel = new Channel(host, port, ChannelCredentials.Insecure);
                    var client = new Handshake.HandShakeExchange.HandShakeExchangeClient(channel);
                    var msg = new Handshake.GreetingMessage
                    {
                        DeviceIdentifier = greetingDevice.DeviceIdentifier.EmptyIfNull(),
                        DeviceName = greetingDevice.DeviceName.EmptyIfNull(),
                        DeviceType = greetingDevice.DeviceType.EmptyIfNull(),
                        Idiom = greetingDevice.Idiom.EmptyIfNull(),
                        Manufacturer = greetingDevice.Manufacturer.EmptyIfNull(),
                        Model = greetingDevice.Model.EmptyIfNull(),
                        OSVersion = greetingDevice.OSVersion.EmptyIfNull(),
                        Platform = greetingDevice.Platform.EmptyIfNull()
                    };
                    var reply = await client.GreetingAsync(msg, cancellationToken: cts.Token, deadline: DateTime.UtcNow.AddSeconds(secondsTimeout));
                    var canConnect = Rediscovery.Shared.Base.Connection.Enums.AllowConnect.None;
                    switch (reply.CanConnect)
                    {
                        case Handshake.GreetingReply.Types.State.None:
                            canConnect = Enums.AllowConnect.None;
                            break;
                        case Handshake.GreetingReply.Types.State.Ok:
                            canConnect = Enums.AllowConnect.OK;
                            break;
                        case Handshake.GreetingReply.Types.State.Error:
                            canConnect = Enums.AllowConnect.Error;
                            break;
                        case Handshake.GreetingReply.Types.State.Denied:
                            canConnect = Enums.AllowConnect.Denied;
                            break;
                        case Handshake.GreetingReply.Types.State.WaitForApprovel:
                            canConnect = Enums.AllowConnect.UnkownDevice;
                            break;
                        default:
                            break;
                    }
                    return new GreetingDeviceReply
                    {
                        SSLPort = reply.SSLPort,
                        PEM = reply.PEM,
                        CanConnect = canConnect,
                        Offline = false,
                        UseSSL = reply.SslActive
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    return new GreetingDeviceReply
                    {
                        SSLPort = -1,
                        PEM = "",
                        CanConnect = Enums.AllowConnect.Error,
                        Offline = false,
                        UseSSL = false
                    };
                }
            });
            var taskTimeout = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(secondsTimeout));
                return new GreetingDeviceReply
                {
                    SSLPort = -1,
                    PEM = "",
                    CanConnect = Enums.AllowConnect.Error,
                    Offline = true,
                    UseSSL = false
                };
            });
            var index = Task.WaitAny(task, taskTimeout);
            channel?.ShutdownAsync().GetAwaiter();
            if (index == 0)
                return task.GetAwaiter().GetResult();
            else
            {
                cts.Cancel();
                return taskTimeout.GetAwaiter().GetResult();
            }
        }
    }
}
