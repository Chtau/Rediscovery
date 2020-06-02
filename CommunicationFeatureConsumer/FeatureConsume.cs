using Featuredata;
using Grpc.Core;
using GrpcService1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationFeatureConsumer
{
    public class FeatureConsume
    {
        public event EventHandler<PluginFeature.Models.DeviceFeatureData> ReceivedFeatureData;
        public event EventHandler<string> HelloReplay;

        private Greeter.GreeterClient client;
        private FeatureExchange.FeatureExchangeClient exchangeClient;
        private IClientStreamWriter<DeviceFeatureData> _responseStream;
        //private ServerCallContext _context;

        public void Connect(string ipAddress, int port, string certificatePEM)
        {
            var channelCredentials = new SslCredentials(certificatePEM);
            Channel channel = new Channel(ipAddress, port, channelCredentials);
            client = new Greeter.GreeterClient(channel);
            exchangeClient = new FeatureExchange.FeatureExchangeClient(channel);
            OnInitFeatureExchange();
        }

        public void SayHello(string value)
        {
            var replay = client.SayHello(new HelloRequest { Name = value });
            HelloReplay?.Invoke(this, replay.Message);
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
        }
    }
}
