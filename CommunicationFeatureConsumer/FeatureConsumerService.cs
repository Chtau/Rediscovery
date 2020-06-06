using CommunicationBase;
using CommunicationBase.Models;
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
        public event EventHandler<PluginFeature.Models.DeviceFeatureData> ReceiveFeatureData;

        private FeatureExchange.FeatureExchangeClient exchangeClient;
        private IClientStreamWriter<DeviceFeatureData> _responseStream;
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

        public void ChangeFeatureState(string token, CommunicationBase.Models.FeatureState featureState)
        {
            Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                try
                {
                    var msg = new Featuredata.FeatureState
                    {
                        FeatureId = featureState.FeatureId,
                        FeatureState_ = (Featuredata.FeatureState.Types.State)(int)featureState.CurrentState
                    };
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    _logger.LogTrace("Consumer send change feature state request");
                    var reply = await exchangeClient.ChangeFeatureStateAsync(msg, cancellationToken: cts.Token, headers: meta);
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
                finally
                {
                    cts.Cancel();
                }
            });
        }

        public void StartFeatureData(string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = exchangeClient.ExchangeStream(headers: meta, cancellationToken: cts.Token))
                    {
                        _responseStream = call.RequestStream;

                        var readTask = Task.Run(async () =>
                        {
                            await foreach (var message in call.ResponseStream.ReadAllAsync())
                            {
                                ReceiveFeatureData?.Invoke(this, new PluginFeature.Models.DeviceFeatureData(message.DeviceId, message.FeatureId.SafeGuid(), message.ProfileId, message.Data));
                            }
                        });
                        do
                        {
                            await Task.Delay(100);
                        } while (!cts.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                finally
                {
                    _responseStream = null;
                    cts.Cancel();
                }
            });
        }

        public void SendFeatureData(PluginFeature.Models.DeviceFeatureData deviceFeatureData)
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
    }
}
