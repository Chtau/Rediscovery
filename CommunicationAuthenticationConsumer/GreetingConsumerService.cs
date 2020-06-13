using Grpc.Core;
using SharedBase.Connection;
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

        public GreetingDeviceReply GreetHost(string host, int port, GreetingDeviceMessage greetingDevice)
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    _logger.LogTrace("Consumer request Greeting");
                    var cts = new CancellationTokenSource();
                    var channel = new Channel(host, port, ChannelCredentials.Insecure);
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
                    var reply = await client.GreetingAsync(msg, cancellationToken: cts.Token);
                    var canConnect = SharedBase.Connection.Enums.AllowConnect.None;
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
                        PEM = reply.PEM,
                        CanConnect = canConnect
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    return new GreetingDeviceReply
                    {
                        PEM = "",
                        CanConnect = Enums.AllowConnect.Error
                    };
                }
            });
            Task.WaitAll(task);
            return task.GetAwaiter().GetResult();
        }
    }
}
