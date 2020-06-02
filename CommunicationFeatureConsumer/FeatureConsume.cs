using Grpc.Core;
using GrpcService1;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureConsumer
{
    public class FeatureConsume
    {
        public event EventHandler<string> HelloReplay;

        private Greeter.GreeterClient client;

        public void Connect(string ipAddress, int port, string certificatePEM)
        {
            var channelCredentials = new SslCredentials(certificatePEM);
            Channel channel = new Channel(ipAddress, port, channelCredentials);
            client = new Greeter.GreeterClient(channel);
        }

        public void SayHello(string value)
        {
            var replay = client.SayHello(new HelloRequest { Name = value });
            HelloReplay?.Invoke(this, replay.Message);
        }
    }
}
