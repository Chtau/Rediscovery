using Featuredata;
using Grpc.Core;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationFeatureConsumer
{
    public class FeatureConsumerService : IFeatureConsumerService
    {
        public event EventHandler<CommunicationBase.Models.FeatureState> ReceiveFeatureStateChangeReply;

        private FeatureExchange.FeatureExchangeClient exchangeClient;
        private readonly ILogger _logger;

        public FeatureConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public void Connect(string ipAddress, int port, string certificatePEM)
        {
            var channelCredentials = new SslCredentials(certificatePEM);
            Channel channel = new Channel(ipAddress, port, channelCredentials);
            exchangeClient = new FeatureExchange.FeatureExchangeClient(channel);
        }

        public void ChangeFeatureState(CommunicationBase.Models.FeatureState featureState)
        {
            Task.Run(async () =>
            {
                try
                {
                    var cts = new CancellationTokenSource();
                    var msg = new FeatureState
                    {
                        FeatureId = featureState.FeatureId,
                        FeatureState_ = (FeatureState.Types.State)(int)featureState.CurrentState
                    };
                    _logger.LogTrace("Consumer send change feature state request");
                    var reply = await exchangeClient.ChangeFeatureStateAsync(msg, cancellationToken: cts.Token);
                    _logger.LogTrace("Consumer reply for feature state change");
                    var replyMsg = new CommunicationBase.Models.FeatureState
                    {
                       CurrentState = (CommunicationBase.Models.FeatureState.State)(int)reply.FeatureState_,
                       FeatureId = reply.FeatureId
                    };
                    ReceiveFeatureStateChangeReply?.Invoke(this, replyMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            });
        }

        /*public void SendFeatureData(PluginFeature.Models.DeviceFeatureData deviceFeatureData)
        {
            if (_responseStream != null)
            {
                Task.Run(async () =>
                {
                    await _responseStream.WriteAsync(new DeviceFeatureData
                    {
                        Data = deviceFeatureData.Data,
                        DeviceId = deviceFeatureData.DeviceId,
                        FeatureId = deviceFeatureData.FeatureId.ToString(),
                        ProfileId = deviceFeatureData.ProfileId
                    });
                });
            }
        }

        private void OnInitFeatureExchange()
        {
            Task.Run(async () =>
            {
                using (var call = exchangeClient.ExchangeStream())
                {
                    _responseStream = call.RequestStream;

                    // Read incoming messages in a background task
                    var readTask = Task.Run(async () =>
                    {
                        await foreach (var message in call.ResponseStream.ReadAllAsync())
                        {
                            ReceivedFeatureData?.Invoke(this, new PluginFeature.Models.DeviceFeatureData(message.DeviceId, message.FeatureId.SafeGuid(), message.ProfileId, message.Data));
                        }
                    });

                    // Finish call and report results
                    await call.RequestStream.CompleteAsync();
                    await readTask;

                    //Console.WriteLine($"Messages sent: {sent}");
                }
            });
        }*/
    }
}
