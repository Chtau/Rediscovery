using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase
{
    public static class ChannelHelper
    {
        public static Grpc.Core.Channel CreateChannel(ConsumerConnectionConfiguration connectionConfiguration)
        {
            if (connectionConfiguration.UseSSL)
            {
                var channelCredentials = new SslCredentials(connectionConfiguration.CertificatePEM);
                return new Channel(connectionConfiguration.IPAddress, connectionConfiguration.SSLPort, channelCredentials);
            }
            else
            {
                return new Channel(connectionConfiguration.IPAddress, connectionConfiguration.Port, ChannelCredentials.Insecure);
            }
        }
    }
}
