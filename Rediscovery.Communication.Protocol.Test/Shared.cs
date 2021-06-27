using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Rediscovery.Communication.Protocol.Test
{

    public static class Shared
    {
        public static IRediscoveryProtocol TestDevice(Models.ConnectionConfiguration connection, 
            Models.ConnectionConfiguration connectionLarge = null,
            Models.ConnectionConfiguration connectionHandshake = null,
            Models.ConnectionConfigurationDiscovery connectionDiscovery = null, bool discoveryListenDeactivated = true)
        {
            IRediscoveryProtocol protocol1 = new RediscoveryProtocol();
            protocol1.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    ListenerDeactivated = discoveryListenDeactivated,
                    Connection = connectionDiscovery ?? new Models.ConnectionConfigurationDiscovery(new List<int> { Models.DiscoveryConfiguration.DefaultListenPort },
                        new List<int> { Models.DiscoveryConfiguration.DefaultSendPort }, Models.DiscoveryConfiguration.DefaultPackageSize)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = connection
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = connectionLarge ?? new Models.ConnectionConfiguration(16578, 16579, 1024 * 60)
                },
                Handshake = new Models.HandshakeConfiguration
                {
                    Connection = connectionHandshake ?? new Models.ConnectionConfiguration(
                        13565,
                        Models.HandshakeConfiguration.DefaultSendPort,
                        Models.HandshakeConfiguration.DefaultPackageSize)
                }
            });
            return protocol1;
        }

        public static IRediscoveryProtocol TestDevice(int portSendOffest = 0, int portListenOffest = 1000)
        {
            IRediscoveryProtocol protocol1 = new RediscoveryProtocol();
            protocol1.Start(new Models.Configuration
            {
                Discovery = new Models.DiscoveryConfiguration
                {
                    Connection = new Models.ConnectionConfigurationDiscovery(new List<int> { Models.DiscoveryConfiguration.DefaultListenPort + portListenOffest }, new List<int> { Models.DiscoveryConfiguration.DefaultSendPort + portSendOffest }, Models.DiscoveryConfiguration.DefaultPackageSize)
                },
                Data = new Models.DataConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(Models.DataConfiguration.DefaultListenPort + portListenOffest, Models.DataConfiguration.DefaultSendPort + portSendOffest, Models.DataConfiguration.DefaultPackageSize)
                },
                Large = new Models.LargeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(Models.LargeConfiguration.DefaultListenPort + portListenOffest, Models.LargeConfiguration.DefaultSendPort + portSendOffest, Models.LargeConfiguration.DefaultPackageSize * 60)
                },
                Handshake = new Models.HandshakeConfiguration
                {
                    Connection = new Models.ConnectionConfiguration(Models.HandshakeConfiguration.DefaultListenPort + portListenOffest, Models.HandshakeConfiguration.DefaultSendPort + portSendOffest, Models.HandshakeConfiguration.DefaultPackageSize)
                }
            });
            return protocol1;
        }
    }
}
